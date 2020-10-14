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

                    var consumed = buffer.Start;
                    var examined = buffer.End;

                    try
                    {
                        if (_parser.TryParse(ref buffer, out var processedLength ))
                        {
                            Logger.Debug($"Buffer length: {buffer.Length}");
                            consumed = processedLength;
                            examined = consumed;
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
                        reader.AdvanceTo(consumed, examined);
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
