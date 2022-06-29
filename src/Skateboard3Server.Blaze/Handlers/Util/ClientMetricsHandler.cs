using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.Util.Messages;

namespace Skateboard3Server.Blaze.Handlers.Util;

public class ClientMetricsHandler : IRequestHandler<ClientMetricsRequest, ClientMetricsResponse>
{
    public Task<ClientMetricsResponse> Handle(ClientMetricsRequest request, CancellationToken cancellationToken)
    {

        var response = new ClientMetricsResponse();
        return Task.FromResult(response);
    }
}