using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.SetGameAttributes)]
[UsedImplicitly]
public record SetGameAttributesRequest : BlazeRequestMessage, IRequest<SetGameAttributesResponse>
{
    [TdfField("ATTR")]
    public Dictionary<string, string> GameAttributes { get; set; }

    [TdfField("GID")]
    public uint GameId { get; set; }
}