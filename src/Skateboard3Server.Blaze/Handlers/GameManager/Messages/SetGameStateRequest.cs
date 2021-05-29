using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeRequest(BlazeComponent.GameManager, (ushort)GameManagerCommand.SetGameState)]
    public class SetGameStateRequest : BlazeRequest, IRequest<SetGameStateResponse>
    {
        [TdfField("GID")]
        public uint GameId { get; set; }

        [TdfField("GSTA")]
        public GameState GameState { get; set; }
    }

}
