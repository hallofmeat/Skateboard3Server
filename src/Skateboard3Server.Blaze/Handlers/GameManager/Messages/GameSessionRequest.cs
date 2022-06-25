using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.GameSession)]
[UsedImplicitly]
public record GameSessionRequest : BlazeRequestMessage, IRequest<GameSessionResponse>
{
    [TdfField("GID")]
    public uint GameId { get; set; }

    [TdfField("XNNC")]
    public byte[] Xnnc { get; set; } //TODO

    [TdfField("XSES")]
    public byte[] Xses { get; set; } //TODO
}