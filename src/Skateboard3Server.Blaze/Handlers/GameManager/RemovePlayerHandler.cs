using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;
using Skateboard3Server.Blaze.Notifications.GameManager;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager
{
    public class RemovePlayerHandler : IRequestHandler<RemovePlayerRequest, RemovePlayerResponse>
    {

        private readonly IBlazeNotificationHandler _notificationHandler;

        public RemovePlayerHandler(IBlazeNotificationHandler notificationHandler)
        {
            _notificationHandler = notificationHandler;
        }

        public async Task<RemovePlayerResponse> Handle(RemovePlayerRequest request, CancellationToken cancellationToken)
        {
            //TODO: currently does not let you leave the session
            //TODO do logic with game manager
            var response = new RemovePlayerResponse();
            //TODO: notify all players
            await _notificationHandler.EnqueueNotification(request.PlayerId, new RemovePlayerNotification
            {
                BlazeErrorCode = 0,
                Cntx = request.Cntx,
                GameId = request.GameId,
                PlayerId = request.PlayerId,
                Reason = request.Reason
            });


            return response;
        }
    }
}