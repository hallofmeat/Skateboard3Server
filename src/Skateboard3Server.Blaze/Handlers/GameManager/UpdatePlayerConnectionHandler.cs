using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;
using Skateboard3Server.Blaze.Notifications.GameManager;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager;

public class UpdatePlayerConnectionHandler : IRequestHandler<UpdatePlayerConnectionRequest, UpdatePlayerConnectionResponse>
{
    private readonly IBlazeNotificationHandler _notificationHandler;

    public UpdatePlayerConnectionHandler(IBlazeNotificationHandler notificationHandler)
    {
        _notificationHandler = notificationHandler;
    }

    public async Task<UpdatePlayerConnectionResponse> Handle(UpdatePlayerConnectionRequest request, CancellationToken cancellationToken)
    {
        var response = new UpdatePlayerConnectionResponse();
        //TODO: update gamemanager
        //TODO: notify all players?
        foreach (var target in request.Targets)
        {
            switch (target.State)
            {
                case PlayerState.Connected:
                    //TODO use context userid?
                    await _notificationHandler.SendNotification(target.PersonaId, new PlayerJoinCompletedNotification
                    {
                        BlazeErrorCode = 0,
                        GameId = request.GameId,
                        PersonaId = target.PersonaId,
                    });
                    break;
                case PlayerState.Disconnected:
                    //TODO use context userid?
                    await _notificationHandler.SendNotification(target.PersonaId, new PlayerRemovedNotification
                    {
                        BlazeErrorCode = 0,
                        Cntx = 0,
                        GameId = request.GameId,
                        PersonaId = target.PersonaId,
                        Reason = PlayerRemoveReason.JoinTimeout //I think this is right?
                    });
                    break;
            }
        }
        return response;
    }
}