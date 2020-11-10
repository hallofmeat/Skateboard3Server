using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Clubs.Messages;

namespace Skate3Server.Blaze.Handlers.Clubs
{
    public class ClubMembershipHandler : IRequestHandler<ClubMembershipRequest, ClubMembershipResponse>
    {
        public async Task<ClubMembershipResponse> Handle(ClubMembershipRequest request, CancellationToken cancellationToken)
        {
            var response = new ClubMembershipResponse();
            return response;
        }
    }
}