using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Stats.Messages
{
    [BlazeResponse(BlazeComponent.Stats, 0x1)]
    public class SkateStatsResponse : BlazeResponse
    {
    }
}
