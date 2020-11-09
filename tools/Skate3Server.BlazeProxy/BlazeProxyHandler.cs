using System;
using System.Buffers;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using NLog;
using Org.BouncyCastle.Crypto.Tls;
using Skate3Server.Blaze;

namespace Skate3Server.BlazeProxy
{
    /// <summary>
    /// Used for proxying to the real server and printing requests (for debug only)
    /// </summary>
    public class BlazeProxyHandler : ConnectionHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IBlazeDebugParser _parser;
        private readonly BlazeProxySettings _proxySettings;

        public BlazeProxyHandler(IBlazeDebugParser parser, IOptions<BlazeProxySettings> proxySettings)
        {
            _parser = parser;
            _proxySettings = proxySettings.Value;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            Logger.Debug($"Connecting to {_proxySettings.RemoteHost}:{_proxySettings.RemotePort}");
            var proxyClient = new TcpClient();
            await proxyClient.ConnectAsync(_proxySettings.RemoteHost, _proxySettings.RemotePort);

            NetworkStream ogStream = proxyClient.GetStream();

            Stream proxyStream = ogStream;

            if (_proxySettings.Secure)
            {
                var protocol = new TlsClientProtocol(proxyStream, new Org.BouncyCastle.Security.SecureRandom());
                protocol.Connect(new BlazeTlsClient());
                proxyStream = protocol.Stream;
            }

            var blazeProtocol = new BlazeProxyProtocol();
            var localReader = connection.CreateReader();
            var localWriter = connection.CreateWriter();

            var remoteReader = new ProtocolReader(proxyStream);
            var remoteWriter = new ProtocolWriter(proxyStream);


            while (true)
            {
                try
                {
                    var result = await localReader.ReadAsync(blazeProtocol);
                    var message = result.Message;

                    if (message != null)
                    {
                        var requestPayload = message.Payload;

                        Logger.Debug($"Parsing Request");

                        if (!_parser.TryParseRequestBody(ref requestPayload))
                        {
                            Logger.Error("Failed to parse request message");
                        }

                        await remoteWriter.WriteAsync(blazeProtocol, message);
                    }

                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                finally
                {
                    localReader.Advance();
                }

                do
                {
                    try
                    {
                        var result = await remoteReader.ReadAsync(blazeProtocol);
                        var message = result.Message;

                        if (message != null)
                        {
                            var responsePayload = message.Payload;

                            Logger.Debug($"Parsing Response");

                            if (!_parser.TryParseResponseBody(ref responsePayload))
                            {
                                Logger.Error("Failed to parse response message");
                            }

                            await localWriter.WriteAsync(blazeProtocol, message);
                        }

                        if (result.IsCompleted)
                        {
                            break;
                        }
                    }
                    finally
                    {
                        remoteReader.Advance();
                    }
                } while (ogStream.DataAvailable);
            }

        }

        private byte[] GetCopyOfSequence(ref ReadOnlySequence<byte> requestBuffer)
        {
            var array = new byte[requestBuffer.Length];
            var requestCopy = new Span<byte>(array);
            requestBuffer.CopyTo(requestCopy);
            return requestCopy.ToArray();
        }
    }
}
