using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using Bedrock.Framework;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Skate3Server.Blaze.Managers;
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
            BlazeClientContext clientContext = null;
            try
            {
                //Create connection scope
                scope = _serviceScopeFactory.CreateScope();
                var messageHandler = scope.ServiceProvider.GetRequiredService<IBlazeMessageHandler>();

                clientContext = (BlazeClientContext) scope.ServiceProvider.GetRequiredService<ClientContext>();
                clientContext.ConnectionContext = connection;
                clientContext.Reader = connection.CreateReader();
                clientContext.Writer = connection.CreateWriter();

                _clientManager.Add(clientContext);

                while (true)
                {
                    try
                    {
                        var result = await clientContext.Reader.ReadAsync(_protocol);
                        var message = result.Message;

                        if (message != null)
                        {
                            var response = await messageHandler.ProcessMessage(message);
                            if (response != null)
                            {
                                await clientContext.Writer.WriteAsync(_protocol, response);
                            }
                            //Send new notifications
                            while (clientContext.PendingNotifications.TryDequeue(out var notification))
                            {
                                await clientContext.Writer.WriteAsync(_protocol, notification);
                            }
                        }

                        if (result.IsCompleted)
                        {
                            break;
                        }
                    }
                    finally
                    {
                        clientContext.Reader.Advance();
                    }
                }
            }
            finally
            {
                if (clientContext != null)
                {
                    _clientManager.Remove(clientContext);
                }
             
                scope?.Dispose();
            }
        }
    }
}
