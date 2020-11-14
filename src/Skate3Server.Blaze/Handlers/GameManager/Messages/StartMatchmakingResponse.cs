using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.GameManager.Messages
{
    [BlazeResponse(BlazeComponent.GameManager, (ushort)GameManagerCommand.StartMatchmaking)]
    public class StartMatchmakingResponse : IBlazeResponse
    {
        [TdfField("MSID")]
        public uint Msid { get; set; } //TODO matchmaking search id?
    }
}
