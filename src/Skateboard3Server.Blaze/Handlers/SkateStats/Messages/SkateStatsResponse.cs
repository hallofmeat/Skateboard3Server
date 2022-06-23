using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.SkateStats.Messages;

[BlazeResponse(BlazeComponent.SkateStats, (ushort)SkateStatsCommand.UpdateStats)]
public class SkateStatsResponse : BlazeResponse
{
    //Empty
}