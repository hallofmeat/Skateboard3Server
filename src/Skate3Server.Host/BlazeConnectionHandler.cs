using System;
using System.IO;
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

                    //We didnt read anything new (probably the end)
                    if (buffer.IsEmpty)
                    {
                        Logger.Warn($"Request buffer empty (connection closed?)");
                        break;
                    }

                    var consumed = buffer.Start;
                    var examined = buffer.End;

                    try
                    {
                        if(_parser.TryParseRequest(ref buffer, out var processedLength, out var header, out var request))
                        {
                            Logger.Debug(
                                $"Request buffer length: {buffer.Length} buffer processed: {processedLength.GetInteger()}");

                            consumed = processedLength;
                            examined = consumed;

                            //TODO: remove stream?
                            await _handler.ProcessRequest(header, request, writer.AsStream());
                            await writer.FlushAsync();
                        }
                        else
                        {
                            Logger.Error("Failed to parse message, trying debug parser");
                            _debugParser.TryParseRequest(ref buffer, out var debugProcessedLength);
                            consumed = debugProcessedLength;
                            examined = consumed;
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
