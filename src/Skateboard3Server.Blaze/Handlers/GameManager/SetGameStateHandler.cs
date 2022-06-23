using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager;

public class SetGameStateHandler : IRequestHandler<SetGameStateRequest, SetGameStateResponse>
{
    private readonly IBlazeNotificationHandler _notificationHandler;
    private readonly IGameManager _gameManager;

    public SetGameStateHandler(IBlazeNotificationHandler notificationHandler, IGameManager gameManager)
    {
        _notificationHandler = notificationHandler;
        _gameManager = gameManager;
    }

    public async Task<SetGameStateResponse> Handle(SetGameStateRequest request, CancellationToken cancellationToken)
    {
        _gameManager.UpdateState(request.GameId, request.GameState);
        //TODO notify other players game settings changed
        var response = new SetGameStateResponse();
        return response;
    }
}