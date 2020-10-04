using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using NLog;
using SkateServer.Blaze;

namespace SkateServer.Host
{
    public class BlazeConnectionHandler : ConnectionHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IBlazeRequestParser _parser;
        private readonly IBlazeRequestHandler _handler;

        public BlazeConnectionHandler(IBlazeRequestParser parser, IBlazeRequestHandler handler)
        {
            _parser = parser;
            _handler = handler;
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

                    SequencePosition examined = buffer.Start;
                    try
                    {
                        if (_parser.TryParseRequest(ref buffer, out var request, out var processedLength))
                        {
                            Logger.Debug($"Buffer length: {buffer.Length} Buffer processed: {processedLength.GetInteger()}");
                            examined = processedLength;

                            //TODO: remove stream?
                            await _handler.ProcessRequest(request, writer.AsStream());
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
                    finally
                    {
                        reader.AdvanceTo(buffer.Start, examined);
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
