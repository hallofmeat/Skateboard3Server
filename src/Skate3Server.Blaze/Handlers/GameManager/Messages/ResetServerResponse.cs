using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeResponse(BlazeComponent.GameManager, (ushort)GameManagerCommand.StartMatchmaking)]
    public class ResetServerResponse : IBlazeResponse
    {
        [TdfField("GID")]
        public uint GameId { get; set; } //TODO
    }
}
