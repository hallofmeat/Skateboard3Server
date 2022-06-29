using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.FinalizeGameCreation)]
[UsedImplicitly]
public record FinalizeGameCreationRequest : BlazeRequestMessage, IRequest<FinalizeGameCreationResponse>
{
    [TdfField("GID")]
    public uint GameId { get; init; }

    [TdfField("XNNC")]
    public byte[] Xnnc { get; init; } //TODO

    [TdfField("XSES")]
    public byte[] Xses { get; init; } //TODO
}