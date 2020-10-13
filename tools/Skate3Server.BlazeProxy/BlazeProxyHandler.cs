using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
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

        private readonly BlazeDebugParser _parser;
        private readonly string _proxyHost;
        private readonly int _proxyPort;
        private readonly bool _proxySecure;

        public BlazeProxyHandler(BlazeDebugParser parser, string proxyHost, int proxyPort, bool proxySecure)
        {
            _parser = parser;
            _proxyHost = proxyHost;
            _proxyPort = proxyPort;
            _proxySecure = proxySecure;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {

            var proxyClient = new TcpClient();
            await proxyClient.ConnectAsync(_proxyHost, _proxyPort);

            Stream proxyStream = proxyClient.GetStream();

            if (_proxySecure)
            {
                var protocol = new TlsClientProtocol(proxyStream, new Org.BouncyCastle.Security.SecureRandom());
                protocol.Connect(new BlazeTlsClient());
                proxyStream = protocol.Stream;
            }

            var reader = connection.Transport.Input;
            var writer = connection.Transport.Output;

            try
            {
                while (true)
                {
                    var tempStream = new MemoryStream();
                    await reader.AsStream().CopyToAsync(tempStream);

                    var requestBytes = tempStream.ToArray();

                    //parse client request
                    var requestSequence = new ReadOnlySequence<byte>(requestBytes);
                    if (_parser.TryParse(ref requestSequence))
                    {
                        Logger.Debug($"Request buffer length: {requestBytes.Length}");
                    }
                    else
                    {
                        Logger.Error("Failed to parse request message");
                    }

                    //send to server
                    await proxyStream.WriteAsync(requestBytes);
                    await proxyStream.FlushAsync();

                    //receive server response
                    var responseBytes = new byte[8192];
                    var bytesRead = await proxyStream.ReadAsync(responseBytes, 0, responseBytes.Length);
                    Logger.Debug($"Response buffer read: {bytesRead}");
                    Array.Resize(ref responseBytes, bytesRead);

                    //parse what was received from server
                    var responseSequence = new ReadOnlySequence<byte>(responseBytes);
                    if (_parser.TryParse(ref responseSequence))
                    {
                        Logger.Debug($"Response buffer length: {responseBytes.Length}");
                    }
                    else
                    {
                        Logger.Error("Failed to parse response message");
                    }

                    //send response to client
                    await writer.AsStream().WriteAsync(responseBytes);
                    await writer.FlushAsync();
                    //TODO: handle connection hangup
                    //TODO: advance reader?
                }
            }
            finally
            {
                await reader.CompleteAsync();
            }
        }
    }
}
