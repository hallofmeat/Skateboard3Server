using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NLog;

namespace Skate3Server.BlazeProxy
{
    public class BlazeProxy
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public async Task Start(string remoteServer, ushort remoteServerPort, ushort localPort)
        {

            var localIpAddress = IPAddress.Any;
            var serverListener = new TcpListener(new IPEndPoint(localIpAddress, localPort));
            serverListener.Server.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            serverListener.Start();

            Logger.Info($"TCP proxy started {localPort} -> {remoteServer}:{remoteServerPort}");
            while (true)
            {

                try
                {
                    var consoleClient = await serverListener.AcceptTcpClientAsync();
                    consoleClient.NoDelay = true;

                    var ips = await Dns.GetHostAddressesAsync(remoteServer);

                    var client = new MirrorTcpClient(consoleClient, new IPEndPoint(ips.First(), remoteServerPort));
                    client.Run();

                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }

            }
        }
    }
}
