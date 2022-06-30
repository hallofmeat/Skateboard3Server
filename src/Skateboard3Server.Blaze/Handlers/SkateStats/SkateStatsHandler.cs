using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.SkateStats.Messages;
using Skateboard3Server.Blaze.Notifications.SkateStats;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.SkateStats;

public class SkateStatsHandler : IRequestHandler<SkateStatsRequest, SkateStatsResponse>
{
    private readonly IBlazeNotificationHandler _notificationHandler;

    public SkateStatsHandler(IBlazeNotificationHandler notificationHandler)
    {
        _notificationHandler = notificationHandler;
    }

    public async Task<SkateStatsResponse> Handle(SkateStatsRequest request, CancellationToken cancellationToken)
    {
        var response = new SkateStatsResponse();

        await _notificationHandler.EnqueueNotification(new SkateStatsReportNotification
        {
            Error = 0,
            Final = false,
            Grid = 0, //always 0
            RequestReport = new RequestReport
            {   //TODO: figure out what these do
                Finished = true,
                Grid = 0, //always 0
                GameType = 1,
                Prcs = false,
                StatsReport = request.StatsReport
            }
        });

        return response;
    }
}