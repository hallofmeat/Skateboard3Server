using System.Threading.Tasks;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Host.Blaze;

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
            clientContext.Connected(connection, connection.CreateWriter(), connection.CreateReader());

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