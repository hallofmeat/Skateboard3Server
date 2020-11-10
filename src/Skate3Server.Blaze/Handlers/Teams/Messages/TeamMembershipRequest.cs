using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Teams.Messages
{
    [BlazeRequest(BlazeComponent.Teams, (ushort)TeamsCommand.TeamMembership)]
    public class TeamMembershipRequest : IRequest<TeamMembershipResponse>
    {
        [TdfField("IDLT")]
        public List<uint> Idlt { get; set; } //TODO

    }
}
