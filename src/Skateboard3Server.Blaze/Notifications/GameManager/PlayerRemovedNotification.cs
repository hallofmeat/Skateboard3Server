using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Notifications.GameManager;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerNotification.PlayerRemoved)]
public record PlayerRemovedNotification : BlazeNotificationMessage
{
    [TdfField("CNTX")]
    public ushort Cntx { get; set; } //TODO context?

    [TdfField("GID")]
    public uint GameId { get; set; }

    [TdfField("PID")]
    public uint PersonaId { get; set; }

    [TdfField("REAS")]
    public PlayerRemoveReason Reason { get; set; }

}