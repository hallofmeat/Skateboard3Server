using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Notifications.GameManager;

[BlazeNotification(BlazeComponent.GameManager, (ushort)GameManagerNotification.MatchmakingFinished)]
public record MatchmakingFinishedNotification : BlazeNotificationMessage
{
    [TdfField("FIT")]
    public uint Fit { get; set; } //TODO

    [TdfField("GID")]
    public uint GameId { get; set; }

    [TdfField("MAXF")]
    public uint Maxf { get; set; } //TODO

    [TdfField("MSID")]
    public uint MatchmakingSessionId { get; set; }

    [TdfField("RSLT")]
    public MatchmakingResult Result { get; set; }

    [TdfField("USID")]
    public uint UserSessionId { get; set; }
}