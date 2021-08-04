using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Notifications.GameManager
{
    [BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerNotification.PlayerJoinCompleted)]
    public class PlayerJoinCompletedNotification : BlazeNotification
    {
        [TdfField("GID")]
        public uint GameId { get; set; }

        [TdfField("PID")]
        public uint PlayerId { get; set; }
    }
}
