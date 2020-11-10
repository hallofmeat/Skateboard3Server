using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Teams.Messages
{
    [BlazeResponse(BlazeComponent.Teams, (ushort)TeamsCommand.Unknown640)]
    public class Unknown640Response : BlazeResponse
    {
    }
}
