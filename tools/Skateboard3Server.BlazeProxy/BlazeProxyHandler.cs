using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Options;
using NLog;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Tls.Crypto.Impl.BC;
using Skateboard3Server.Blaze;

namespace Skateboard3Server.BlazeProxy;

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

        var ogStream = proxyClient.GetStream();

        Stream proxyStream = ogStream;

        if (_proxySettings.Secure)
        {
            var protocol = new TlsClientProtocol(proxyStream);
            protocol.Connect(new BlazeTlsClient(new BcTlsCrypto(new SecureRandom())));
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
                    var header = message.Header;
                    Logger.Debug(
                        $"Client -> Proxy; Length:{header.Length} Component:{header.Component} Command:0x{header.Command:X2} ErrorCode:{header.ErrorCode} MessageType:{header.MessageType} MessageId:{header.MessageId}");

                    var requestPayload = message.Payload;

                    if (!requestPayload.IsEmpty)
                    {
                        if (!_parser.TryParseBody(requestPayload))
                        {
                            Logger.Error("Failed to parse request message");
                        }
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
                        var header = message.Header;
                        Logger.Debug(
                            $"Proxy <- Server; Length:{header.Length} Component:{header.Component} Command:0x{header.Command:X2} ErrorCode:{header.ErrorCode} MessageType:{header.MessageType} MessageId:{header.MessageId}");

                        var responsePayload = message.Payload;

                        if (!responsePayload.IsEmpty)
                        {
                            if (!_parser.TryParseBody(responsePayload))
                            {
                                Logger.Error("Failed to parse response message");
                            }
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
}