using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeResponse(BlazeComponent.GameManager, (ushort)GameManagerCommand.CreateGame)]
    public class CreateGameResponse : BlazeResponse
    {
        [TdfField("GID")]
        public uint GameId { get; set; }
    }
}
