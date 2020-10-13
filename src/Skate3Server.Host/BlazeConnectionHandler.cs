using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using NLog;
using Skate3Server.Blaze;

namespace Skate3Server.Host
{
    public class BlazeConnectionHandler : ConnectionHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IBlazeRequestParser _parser;
        //private readonly BlazeDebugParser _debugParser;
        private readonly IBlazeRequestHandler _handler;
        private readonly IBlazeDebugParser _debugParser;

        public BlazeConnectionHandler(IBlazeRequestParser parser, IBlazeRequestHandler handler, IBlazeDebugParser debugParser)
        {
            _parser = parser;
           // _debugParser = debugParser;
            _handler = handler;
            _debugParser = debugParser;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            //https://devblogs.microsoft.com/dotnet/system-io-pipelines-high-performance-io-in-net/
            //https://github.com/davidfowl/MultiProtocolAspNetCore
            //https://docs.microsoft.com/en-us/dotnet/standard/io/pipelines

            var reader = connection.Transport.Input;
            var writer = connection.Transport.Output;

            try
            {
                while (true)
                {
                    var result = await reader.ReadAsync();
                    var buffer = result.Buffer;

                    SequencePosition consumed = buffer.Start;
                    SequencePosition examined = buffer.End;

                    try
                    {
                        if (_parser.TryParseRequest(ref buffer, out var processedLength, out var header, out var request))
                        {
                            Logger.Debug(
                                $"Buffer length: {buffer.Length} Buffer processed: {processedLength.GetInteger()}");
                            //examined = processedLength;

                            consumed = buffer.Start;
                            examined = consumed;

                            //TODO: remove stream?
                            await _handler.ProcessRequest(header, request, writer.AsStream());
                            await writer.FlushAsync();
                        }
                        else
                        {
                            Logger.Error("Failed to parse message, trying debug parser");
                            _debugParser.TryParse(ref buffer);
                            break;
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
