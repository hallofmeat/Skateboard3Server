using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using NLog;
using Skate3Server.Blaze;

namespace Skate3Server.Host
{
    /// <summary>
    /// Used for using BlazeDebugParser
    /// </summary>
    public class BlazeDebugHandler : ConnectionHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly BlazeDebugParser _parser;

        public BlazeDebugHandler(BlazeDebugParser parser)
        {
            _parser = parser;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
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
