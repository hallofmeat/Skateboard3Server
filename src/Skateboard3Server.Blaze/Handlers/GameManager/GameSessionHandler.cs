using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;

namespace Skateboard3Server.Blaze.Handlers.GameManager;

public class GameSessionHandler : IRequestHandler<GameSessionRequest, GameSessionResponse>
{
    public async Task<GameSessionResponse> Handle(GameSessionRequest request, CancellationToken cancellationToken)
    {
        var response = new GameSessionResponse();
        return response;
    }
}