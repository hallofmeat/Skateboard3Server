using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Notifications.UserSession;

[BlazeNotification(BlazeComponent.UserSession, (ushort)UserSessionNotification.UserExtendedData)]
public record UserExtendedDataNotification : BlazeNotificationMessage
{
    [TdfField("DATA")]
    public UserExtendedData Data { get; set; }

    [TdfField("USID")]
    public uint UserId { get; set; }
}