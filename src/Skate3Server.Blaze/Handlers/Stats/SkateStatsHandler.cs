using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Stats.Messages;
using Skate3Server.Blaze.Notifications.UserSession.Messages;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Stats
{
    public class SkateStatsHandler : IRequestHandler<SkateStatsRequest, SkateStatsResponse>
    {
        public async Task<SkateStatsResponse> Handle(SkateStatsRequest request, CancellationToken cancellationToken)
        {
            var response = new SkateStatsResponse();

            response.Notifications.Add(new BlazeHeader
            {
                Component = BlazeComponent.Stats,
                Command = 0x72,
                MessageId = 0,
                MessageType = BlazeMessageType.Notification,
                ErrorCode = 0
            }, new SkateStatsNotification
            {
                Error = 0,
                Final = false,
                Grid = 0,
                RequestReport = new RequestReport
                {   //TODO: figure out what these do
                    Finished = true,
                    Grid = 0,
                    GType = 1,
                    Prcs = false,
                    StatsReport = request.StatsReport
                }
            });

            return response;
        }
    }
}