using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Clubs.Messages
{
    [BlazeResponse(BlazeComponent.Clubs, (ushort)ClubsCommand.ClubMembership)]
    public class ClubMembershipResponse : BlazeResponse
    {
    }
}
