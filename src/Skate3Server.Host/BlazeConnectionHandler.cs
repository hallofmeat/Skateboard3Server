using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Host
{
    public class BlazeConnectionHandler : ConnectionHandler
    {
        private readonly BlazeProtocol _protocol;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IClientManager _clientManager;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public BlazeConnectionHandler(BlazeProtocol protocol, IServiceScopeFactory serviceScopeFactory, IClientManager clientManager)
        {
            _protocol = protocol;
            _serviceScopeFactory = serviceScopeFactory;
            _clientManager = clientManager;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            IServiceScope scope = null;

            try
            {
                //Create connection scope
                scope = _serviceScopeFactory.CreateScope();
                var messageHandler = scope.ServiceProvider.GetRequiredService<IBlazeMessageHandler>();

                var clientContext = (BlazeClientContext) scope.ServiceProvider.GetRequiredService<ClientContext>();
                clientContext.ConnectionContext = connection;
                _clientManager.Add(clientContext);

                var reader = connection.CreateReader();
                var writer = connection.CreateWriter();

                while (true)
                {
                    try
                    {
                        var result = await reader.ReadAsync(_protocol);
                        var message = result.Message;

                        if (message != null)
                        {
                            var responses = await messageHandler.ProcessMessage(message);
                            if (responses != null)
                            {
                                foreach (var response in responses)
                                {
                                    await writer.WriteAsync(_protocol, response);
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
            finally
            {
                scope?.Dispose();
            }
        }
    }
}
