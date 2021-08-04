using System.Collections.Generic;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.UpdatePlayerConnection)]
    public class UpdatePlayerConnectionRequest : BlazeRequest, IRequest<UpdatePlayerConnectionResponse>
    {
        [TdfField("GID")]
        public uint GameId { get; set; }

        [TdfField("TARG")]
        public List<PlayerTarget> Targets { get; set; }
    }

    public class PlayerTarget
    {
        [TdfField("PID")]
        public uint PlayerId { get; set; }

        [TdfField("STAT")]
        public PlayerState State { get; set; }
    }

}
