using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Util.Messages;

namespace Skate3Server.Blaze.Handlers.Util
{
    public class PostAuthHandler : IRequestHandler<PostAuthRequest, PostAuthResponse>
    {
        public async Task<PostAuthResponse> Handle(PostAuthRequest request, CancellationToken cancellationToken)
        {
            var response = new PostAuthResponse
            {
            };
            return response;
        }
    }
}