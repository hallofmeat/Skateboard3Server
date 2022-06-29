using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.RemovePlayer)]
[UsedImplicitly]
public record RemovePlayerRequest : BlazeRequestMessage, IRequest<RemovePlayerResponse>
{
    [TdfField("BTPL")]
    public ulong Btpl { get; init; } //TODO

    [TdfField("CNTX")]
    public ushort Cntx { get; init; } //TODO context?

    [TdfField("GID")]
    public uint GameId { get; init; }

    [TdfField("PID")]
    public uint PersonaId { get; init; }

    [TdfField("REAS")]
    public PlayerRemoveReason Reason { get; init; }

}