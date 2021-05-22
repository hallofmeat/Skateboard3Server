using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;

namespace Skateboard3Server.Blaze.Handlers.GameManager
{
    public class SetGameAttributesHandler : IRequestHandler<SetGameAttributesRequest, SetGameAttributesResponse>
    {
        public async Task<SetGameAttributesResponse> Handle(SetGameAttributesRequest request, CancellationToken cancellationToken)
        {
            var response = new SetGameAttributesResponse();
            return response;
        }
    }
}