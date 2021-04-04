using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeResponse(BlazeComponent.GameManager, (ushort)GameManagerCommand.StartMatchmaking)]
    public class ResetServerResponse : IBlazeResponse
    {
        [TdfField("GID")]
        public uint GameId { get; set; } //TODO
    }
}
