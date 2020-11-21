using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.SkateStats.Messages
{
    [BlazeResponse(BlazeComponent.SkateStats, (ushort)SkateStatsCommand.UpdateStats)]
    public class SkateStatsResponse : IBlazeResponse
    {
        //Empty
    }
}
