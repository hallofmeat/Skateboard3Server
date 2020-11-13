using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Teams.Messages
{
    [BlazeRequest(BlazeComponent.Teams, (ushort)TeamsCommand.TeamMembership)]
    public class TeamMembershipRequest : IRequest<TeamMembershipResponse>, IBlazeRequest
    {
        [TdfField("IDLT")]
        public List<uint> Idlt { get; set; } //TODO

    }
}
