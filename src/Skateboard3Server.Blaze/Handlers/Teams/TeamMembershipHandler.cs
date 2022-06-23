using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.Teams.Messages;

namespace Skateboard3Server.Blaze.Handlers.Teams;

public class TeamMembershipHandler : IRequestHandler<TeamMembershipRequest, TeamMembershipResponse> //TODO: I think this name is wrong
{
    public async Task<TeamMembershipResponse> Handle(TeamMembershipRequest request, CancellationToken cancellationToken)
    {
        var response = new TeamMembershipResponse();
        return response;
    }
}