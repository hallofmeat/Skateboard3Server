using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.UpdatePlayerConnection)]
[UsedImplicitly]
public record UpdatePlayerConnectionRequest : BlazeRequestMessage, IRequest<UpdatePlayerConnectionResponse>
{
    [TdfField("GID")]
    public uint GameId { get; init; }

    [TdfField("TARG")]
    public List<PlayerTarget> Targets { get; init; }
}

public record PlayerTarget
{
    [TdfField("PID")]
    public uint PersonaId { get; init; }

    [TdfField("STAT")]
    public PlayerState State { get; init; }
}