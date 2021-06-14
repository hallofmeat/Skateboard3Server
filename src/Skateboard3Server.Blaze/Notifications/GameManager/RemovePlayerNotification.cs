using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Notifications.GameManager
{
    [BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerNotification.RemovePlayer)]
    public class RemovePlayerNotification : BlazeNotification
    {
        [TdfField("CNTX")]
        public ushort Cntx { get; set; } //TODO

        [TdfField("GID")]
        public uint GameId { get; set; }

        [TdfField("PID")]
        public uint PlayerId { get; set; }

        [TdfField("REAS")]
        public int Reason { get; set; } //TODO enum

    }
}
