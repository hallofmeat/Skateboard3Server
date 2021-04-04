using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;

namespace Skateboard3Server.Blaze.Handlers.GameManager
{
    public class GameAttributesHandler : IRequestHandler<GameAttributesRequest, GameAttributesResponse>
    {
        public async Task<GameAttributesResponse> Handle(GameAttributesRequest request, CancellationToken cancellationToken)
        {
            var response = new GameAttributesResponse();
            return response;
        }
    }
}