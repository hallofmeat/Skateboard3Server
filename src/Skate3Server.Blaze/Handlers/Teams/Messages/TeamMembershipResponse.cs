using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Teams.Messages
{
    [BlazeResponse(BlazeComponent.Teams, (ushort)TeamsCommand.TeamMembership)]
    public class TeamMembershipResponse : IBlazeResponse
    {
        //Empty
    }
}
