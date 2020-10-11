using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Authentication.Messages;

namespace Skate3Server.Blaze.Handlers.Authentication
{
    public class PreAuthHandler : IRequestHandler<PreAuthRequest, PreAuthResponse>
    {
        public async Task<PreAuthResponse> Handle(PreAuthRequest request, CancellationToken cancellationToken)
        {
            var response = new PreAuthResponse
            {
            };
            return response;
        }
    }
}