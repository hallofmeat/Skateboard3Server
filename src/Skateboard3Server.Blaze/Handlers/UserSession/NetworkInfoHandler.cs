using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.UserSession.Messages;

namespace Skateboard3Server.Blaze.Handlers.UserSession
{
    public class NetworkInfoHandler : IRequestHandler<NetworkInfoRequest, NetworkInfoResponse>
    {
        public async Task<NetworkInfoResponse> Handle(NetworkInfoRequest request, CancellationToken cancellationToken)
        {
            var response = new NetworkInfoResponse();
            return response;
        }
    }
}