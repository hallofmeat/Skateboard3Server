using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;
using Skateboard3Server.Blaze.Managers;

namespace Skateboard3Server.Blaze.Handlers.GameManager;

public class SetGameSettingsHandler : IRequestHandler<SetGameSettingsRequest, SetGameSettingsResponse>
{
    private readonly IGameManager _gameManager;

    public SetGameSettingsHandler(IGameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public Task<SetGameSettingsResponse> Handle(SetGameSettingsRequest request, CancellationToken cancellationToken)
    {
        _gameManager.UpdateSettings(request.GameId, request.GameSettings);
        //TODO notify other players game settings changed
        var response = new SetGameSettingsResponse();
        return Task.FromResult(response);
    }
}