using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NLog;
using Org.BouncyCastle.Crypto.Tls;

namespace Skate3Server.BlazeProxy
{
    class MirrorTcpClient
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly TcpClient _consoleClient;
        private readonly IPEndPoint _clientEndpoint;
        private readonly IPEndPoint _remoteServer;

        private readonly TcpClient _wrappedClient;

        public MirrorTcpClient(TcpClient consoleClient, IPEndPoint remoteServer)
        {
            _consoleClient = consoleClient;
            _remoteServer = remoteServer;

            _wrappedClient = new TcpClient { NoDelay = true };

            _clientEndpoint = (IPEndPoint)_consoleClient.Client.RemoteEndPoint;
            Logger.Info($"Established {_clientEndpoint} => {remoteServer}");
        }

        public void Run()
        {
            Task.Run(async () =>
            {
                try
                {
                    using (_consoleClient)
                    using (_wrappedClient)
                    {
                        await _wrappedClient.ConnectAsync(_remoteServer.Address, _remoteServer.Port);

                        var protocol = new TlsClientProtocol(_wrappedClient.GetStream(), new Org.BouncyCastle.Security.SecureRandom());
                        protocol.Connect(new BlazeTlsClient());

                        var serverStream = protocol.Stream;
                        var remoteStream = _consoleClient.GetStream();

                        await Task.WhenAny(remoteStream.CopyToAsync(serverStream), serverStream.CopyToAsync(remoteStream));
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
                finally
                {
                    Logger.Info($"Closed {_clientEndpoint} => {_remoteServer}");
                }
            });
        }
    }
}