using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.SetGameSettings)]
[UsedImplicitly]
public record SetGameSettingsRequest : BlazeRequestMessage, IRequest<SetGameSettingsResponse>
{
    [TdfField("GID")]
    public uint GameId { get; init; }

    [TdfField("GSET")]
    public uint GameSettings { get; init; }
}