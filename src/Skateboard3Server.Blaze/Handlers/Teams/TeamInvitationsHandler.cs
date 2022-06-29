using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.Teams.Messages;

namespace Skateboard3Server.Blaze.Handlers.Teams;

public class TeamInvitationsHandler : IRequestHandler<TeamInvitationsRequest, TeamInvitationsResponse>
{
    public Task<TeamInvitationsResponse> Handle(TeamInvitationsRequest request, CancellationToken cancellationToken)
    {
        var response = new TeamInvitationsResponse();
        return Task.FromResult(response);
    }
}