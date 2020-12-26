using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.GameManager.Messages;

namespace Skate3Server.Blaze.Handlers.GameManager
{
    public class ResetServerHandler : IRequestHandler<ResetServerRequest, ResetServerResponse>
    {
        public async Task<ResetServerResponse> Handle(ResetServerRequest request, CancellationToken cancellationToken)
        {
            var response = new ResetServerResponse
            {
                GameId = 12345 //TODO
            };
            return response;
        }
    }
}