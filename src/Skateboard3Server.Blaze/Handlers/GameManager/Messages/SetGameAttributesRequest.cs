using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.SetGameAttributes)]
[UsedImplicitly]
public record SetGameAttributesRequest : BlazeRequestMessage, IRequest<SetGameAttributesResponse>
{
    [TdfField("ATTR")]
    public Dictionary<string, string> GameAttributes { get; init; }

    [TdfField("GID")]
    public uint GameId { get; init; }
}