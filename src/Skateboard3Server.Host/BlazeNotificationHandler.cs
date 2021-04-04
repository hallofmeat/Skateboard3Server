using System.Buffers;
using System.IO;
using System.Threading.Tasks;
using NLog;
using Skateboard3Server.Blaze;
using Skateboard3Server.Blaze.Serializer;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Host
{
    public class BlazeNotificationHandler : IBlazeNotificationHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly IBlazeSerializer _blazeSerializer;
        private readonly IBlazeTypeLookup _blazeTypeLookup;
        private readonly IClientManager _clientManager;

        private readonly BlazeProtocol _protocol;

        public BlazeNotificationHandler(BlazeProtocol protocol, IClientManager clientManager,
            IBlazeTypeLookup blazeTypeLookup, IBlazeSerializer blazeSerializer)
        {
            _protocol = protocol;
            _clientManager = clientManager;
            _blazeTypeLookup = blazeTypeLookup;
            _blazeSerializer = blazeSerializer;
        }

        //TODO: add self? (notifications to the current user)

        public async Task EnqueueNotification(uint userId, IBlazeNotification notification, ushort errorCode = 0)
        {
            var userContext = _clientManager.GetByUserId(userId) as BlazeClientContext;
            if (userContext == null)
            {
                Logger.Warn($"User {userId} is not connected");
                return;
            }

            var messageData = EncodeNotification(notification, errorCode);

            userContext.PendingNotifications.Enqueue(messageData);
        }

        public async Task SendNotification(uint userId, IBlazeNotification notification, ushort errorCode = 0)
        {
            var userContext = _clientManager.GetByUserId(userId) as BlazeClientContext;
            if (userContext == null)
            {
                Logger.Warn($"User {userId} is not connected");
                return;
            }

            var messageData = EncodeNotification(notification, errorCode);

            if (userContext.Writer != null)
            {
                await userContext.Writer.WriteAsync(_protocol, messageData);
            }
        }

        private BlazeMessageData EncodeNotification(IBlazeNotification notification, ushort errorCode)
        {
            _blazeTypeLookup.TryGetResponseComponentCommand(notification.GetType(), out var component, out var command);

            var header = new BlazeHeader
            {
                Component = component,
                Command = command,
                MessageId = 0, //always 0
                MessageType = BlazeMessageType.Notification,
                ErrorCode = errorCode
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
}