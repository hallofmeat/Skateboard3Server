using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
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
            //https://docs.microsoft.com/en-us/dotnet/standard/io/pipelines#reading-multiple-messages

            var reader = connection.Transport.Input;
            var writer = connection.Transport.Output;

            try
            {
                while (true)
                {
                    var result = await reader.ReadAsync();
                    var buffer = result.Buffer;

                    try
                    {
                        while (_parser.TryParseRequest(ref buffer, out var request))
                        {
                            //TODO: remove stream?
                            await _handler.ProcessRequest(request, writer.AsStream());
                        }

                        if (result.IsCompleted)
                        {
                            if (buffer.Length > 0)
                            {
                                throw new InvalidDataException("Incomplete request");
                            }
                            break;
                        }
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
