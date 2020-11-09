using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using NLog;
using Skate3Server.Blaze;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Host
{
    public class BlazeConnectionHandler : ConnectionHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IBlazeMessageHandler _handler;

        public BlazeConnectionHandler(IBlazeMessageHandler handler)
        {
            _handler = handler;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            //TODO: DI
            var protocol = new BlazeProtocol();
            var reader = connection.CreateReader();
            var writer = connection.CreateWriter();

            while (true)
            {
                try
                {
                    var result = await reader.ReadAsync(protocol);
                    var message = result.Message;

                    if (message != null)
                    {
                        var responses = await _handler.ProcessMessage(message);
                        if (responses != null)
                        {
                            foreach (var response in responses)
                            {
                                await writer.WriteAsync(protocol, response);
                            }
                        }
                    }

                    if (result.IsCompleted)
                    {
                        break;
                    }
                }
                finally
                {
                    reader.Advance();
                }
            }
        }
    }
}
