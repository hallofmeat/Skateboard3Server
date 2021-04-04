using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;

namespace Skateboard3Server.Blaze.Handlers.GameManager
{
    public class FinalizeGameCreationHandler : IRequestHandler<FinalizeGameCreationRequest, FinalizeGameCreationResponse>
    {
        public async Task<FinalizeGameCreationResponse> Handle(FinalizeGameCreationRequest request, CancellationToken cancellationToken)
        {
            var response = new FinalizeGameCreationResponse();
            return response;
        }
    }
}