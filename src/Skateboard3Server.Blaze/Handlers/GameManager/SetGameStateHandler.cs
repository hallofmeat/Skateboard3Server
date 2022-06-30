using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;
using Skateboard3Server.Blaze.Managers;

namespace Skateboard3Server.Blaze.Handlers.GameManager;

public class SetGameStateHandler : IRequestHandler<SetGameStateRequest, SetGameStateResponse>
{
    private readonly IGameManager _gameManager;

    public SetGameStateHandler(IGameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public Task<SetGameStateResponse> Handle(SetGameStateRequest request, CancellationToken cancellationToken)
    {
        _gameManager.UpdateState(request.GameId, request.GameState);
        //TODO notify other players game settings changed
        var response = new SetGameStateResponse();
        return Task.FromResult(response);
    }
}