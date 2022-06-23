using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.UserSession.Messages;

namespace Skateboard3Server.Blaze.Handlers.UserSession;

public class HardwareFlagsHandler : IRequestHandler<HardwareFlagsRequest, HardwareFlagsResponse>
{
    public async Task<HardwareFlagsResponse> Handle(HardwareFlagsRequest request, CancellationToken cancellationToken)
    {
        var response = new HardwareFlagsResponse();
        return response;
    }
}