using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using NLog;
using SkateServer.Blaze;

namespace SkateServer.Host
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
            //TODO: create connection to proxy server

            var reader = connection.Transport.Input;

            try
            {
                while (true)
                {
                    var result = await reader.ReadAsync();
                    var buffer = result.Buffer;

                    try
                    {
                        if (_parser.TryParse(ref buffer))
                        {
                            Logger.Debug($"Buffer length: {buffer.Length}");
                        }
                        else
                        {
                            Logger.Error("Failed to parse message");
                        }

                        //TODO: send to proxy server
                        //TODO: parse proxy server response

                        if (result.IsCompleted)
                        {
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error($"Failed to handle request: {e}");
                        break;
                    }
                    finally
                    {
                        reader.AdvanceTo(buffer.Start, buffer.End);
                    }
                }
            }
            finally
            {
                await reader.CompleteAsync();
            }
        }
    }
}
