using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.GameAttributes)]
    public class GameAttributesRequest : IRequest<GameAttributesResponse>, IBlazeRequest
    {
        [TdfField("ATTR")]
        public Dictionary<string, string> Attributes { get; set; }

        [TdfField("GID")]
        public uint GameId { get; set; }
    }

}
