﻿using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Notifications.GameManager;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerNotification.PlayerJoinCompleted)]
public record PlayerJoinCompletedNotification : BlazeNotificationMessage
{
    [TdfField("GID")]
    public uint GameId { get; set; }

    [TdfField("PID")]
    public uint PersonaId { get; set; }
}