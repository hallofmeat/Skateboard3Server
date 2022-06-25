using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager.Messages;

[BlazeResponse(BlazeComponent.GameManager, (ushort)GameManagerCommand.StartMatchmaking)]
public record StartMatchmakingResponse : BlazeResponseMessage
{
    [TdfField("MSID")]
    public uint MatchmakingSessionId { get; set; }
}