using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.Teams.Messages;

namespace Skateboard3Server.Blaze.Handlers.Teams;

//Contacts List -> Teammates List
public class TeamMembersHandler : IRequestHandler<TeamMembersRequest, TeamMembersResponse>
{
    public Task<TeamMembersResponse> Handle(TeamMembersRequest request, CancellationToken cancellationToken)
    {
        var response = new TeamMembersResponse();
        response.BlazeErrorCode = 1009; //not on a team?
        return Task.FromResult(response);
    }
}