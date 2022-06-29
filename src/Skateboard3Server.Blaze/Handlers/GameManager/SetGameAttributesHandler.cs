using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager;

public class SetGameAttributesHandler : IRequestHandler<SetGameAttributesRequest, SetGameAttributesResponse>
{
    private readonly IBlazeNotificationHandler _notificationHandler;
    private readonly IGameManager _gameManager;

    public SetGameAttributesHandler(IBlazeNotificationHandler notificationHandler, IGameManager gameManager)
    {
        _notificationHandler = notificationHandler;
        _gameManager = gameManager;
    }

    public Task<SetGameAttributesResponse> Handle(SetGameAttributesRequest request, CancellationToken cancellationToken)
    {
        _gameManager.UpdateAttributes(request.GameId, request.GameAttributes);
        //TODO notify all other users in a game
        var response = new SetGameAttributesResponse();
        return Task.FromResult(response);
    }
}