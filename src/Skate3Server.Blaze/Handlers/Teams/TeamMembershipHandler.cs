using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Teams.Messages;

namespace Skate3Server.Blaze.Handlers.Teams
{
    public class TeamMembershipHandler : IRequestHandler<TeamMembershipRequest, TeamMembershipResponse>
    {
        public async Task<TeamMembershipResponse> Handle(TeamMembershipRequest request, CancellationToken cancellationToken)
        {
            var response = new TeamMembershipResponse();
            return response;
        }
    }
}