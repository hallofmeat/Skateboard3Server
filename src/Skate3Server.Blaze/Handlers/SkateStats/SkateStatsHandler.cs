using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.SkateStats.Messages;
using Skate3Server.Blaze.Notifications.SkateStats;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.SkateStats
{
    public class SkateStatsHandler : IRequestHandler<SkateStatsRequest, SkateStatsResponse>
    {
        private readonly ClientContext _clientContext;

        public SkateStatsHandler(ClientContext clientContext)
        {
            _clientContext = clientContext;
        }

        public async Task<SkateStatsResponse> Handle(SkateStatsRequest request, CancellationToken cancellationToken)
        {
            var response = new SkateStatsResponse();

            _clientContext.Notifications.Enqueue((new BlazeHeader
            {
                Component = BlazeComponent.SkateStats,
                Command = (ushort)SkateStatsNotification.StatsReport,
                MessageId = 0,
                MessageType = BlazeMessageType.Notification,
                ErrorCode = 0
            }, new SkateStatsReportNotification
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
            }));

            return response;
        }
    }
}