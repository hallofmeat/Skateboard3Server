using System.Collections.Generic;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages;

[BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.SetGameAttributes)]
public class SetGameAttributesRequest : BlazeRequest, IRequest<SetGameAttributesResponse>
{
    [TdfField("ATTR")]
    public Dictionary<string, string> GameAttributes { get; set; }

    [TdfField("GID")]
    public uint GameId { get; set; }
}