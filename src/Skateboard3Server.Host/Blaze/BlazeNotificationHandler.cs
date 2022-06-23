using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using Bedrock.Framework;
using NLog;
using Skateboard3Server.Blaze;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Serializer;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Host.Blaze;

public class BlazeNotificationHandler : IBlazeNotificationHandler
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private readonly IBlazeSerializer _blazeSerializer;
    private readonly IBlazeTypeLookup _blazeTypeLookup;
    private readonly IClientManager _clientManager;

    private readonly BlazeProtocol _protocol;
    private readonly ClientContext _clientContext;

    public BlazeNotificationHandler(BlazeProtocol protocol, ClientContext clientContext, IClientManager clientManager,
        IBlazeTypeLookup blazeTypeLookup, IBlazeSerializer blazeSerializer)
    {
        _protocol = protocol;
        _clientContext = clientContext;
        _clientManager = clientManager;
        _blazeTypeLookup = blazeTypeLookup;
        _blazeSerializer = blazeSerializer;
    }

    public Task EnqueueNotification(uint userId, BlazeNotification notification)
    {
        var userContext = _clientManager.GetByUserId(userId) as BlazeClientContext;
        if (userContext == null)
        {
            Logger.Warn($"User {userId} is not connected");
            return Task.CompletedTask;
        }

        var messageData = EncodeNotification(notification);
        userContext.PendingNotifications.Enqueue(messageData);
        return Task.CompletedTask;
    }

    public Task EnqueueNotification(BlazeNotification notification)
    {
        var currentUserContext = _clientContext as BlazeClientContext;
        if (currentUserContext == null)
        {
            Logger.Warn($"Current User has no context");
            return Task.CompletedTask;
        }

        var messageData = EncodeNotification(notification);
        currentUserContext.PendingNotifications.Enqueue(messageData);
        return Task.CompletedTask;
    }

    public async Task SendNotification(uint userId, BlazeNotification notification)
    {
        var userContext = _clientManager.GetByUserId(userId) as BlazeClientContext;
        if (userContext == null)
        {
            Logger.Warn($"User {userId} is not connected");
            return;
        }

        var messageData = EncodeNotification(notification);

        if (userContext.Writer != null)
        {
            await userContext.Writer.WriteAsync(_protocol, messageData);
        }
    }

    public async Task SendNotification(BlazeNotification notification)
    {
        var currentUserContext = _clientContext as BlazeClientContext;
        if (currentUserContext == null)
        {
            Logger.Warn($"Current User has no context");
            return;
        }

        var messageData = EncodeNotification(notification);

        if (currentUserContext.Writer != null)
        {
            await currentUserContext.Writer.WriteAsync(_protocol, messageData);
        }
    }

    private BlazeMessageData EncodeNotification(BlazeNotification notification)
    {
        _blazeTypeLookup.TryGetResponseComponentCommand(notification.GetType(), out var component, out var command);

        var header = new BlazeHeader
        {
            Component = component,
            Command = command,
            MessageId = 0, //always 0
            MessageType = BlazeMessageType.Notification,
            ErrorCode = notification.BlazeErrorCode
        };

        //TODO: remove stream
        var notificationOutput = new MemoryStream();
        _blazeSerializer.Serialize(notificationOutput, notification);
        var messageData = new BlazeMessageData
        {
            Header = header,
            Payload = new ReadOnlySequence<byte>(notificationOutput.ToArray())
        };
        return messageData;
    }
}