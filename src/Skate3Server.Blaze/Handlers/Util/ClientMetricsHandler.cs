using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Util.Messages;

namespace Skate3Server.Blaze.Handlers.Util
{
    public class ClientMetricsHandler : IRequestHandler<ClientMetricsRequest, ClientMetricsResponse>
    {
        public async Task<ClientMetricsResponse> Handle(ClientMetricsRequest request, CancellationToken cancellationToken)
        {

            var response = new ClientMetricsResponse();
            return response;
        }
    }
}