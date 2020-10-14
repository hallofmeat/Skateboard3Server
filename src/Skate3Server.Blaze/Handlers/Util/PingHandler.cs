using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Util.Messages;

namespace Skate3Server.Blaze.Handlers.Util
{
    public class PingHandler : IRequestHandler<PingRequest, PingResponse>
    {
        public async Task<PingResponse> Handle(PingRequest request, CancellationToken cancellationToken)
        {

            var response = new PingResponse
            {
                //https://stackoverflow.com/a/17632585
                Timestamp = (uint)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds
            };
            return response;
        }
    }
}