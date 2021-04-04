using System.Collections.Generic;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Teams.Messages
{
    [BlazeRequest(BlazeComponent.Teams, (ushort)TeamsCommand.TeamMembership)]
    public class TeamMembershipRequest : IRequest<TeamMembershipResponse>, IBlazeRequest
    {
        [TdfField("IDLT")]
        public List<uint> Idlt { get; set; } //TODO

    }
}
