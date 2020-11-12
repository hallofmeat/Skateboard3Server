using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Handlers.Util.Messages;

namespace Skate3Server.Blaze.Handlers.Util
{
    public class PingHandler : IRequestHandler<PingRequest, PingResponse>
    {
        public async Task<PingResponse> Handle(PingRequest request, CancellationToken cancellationToken)
        {

            var response = new PingResponse
            {
                Timestamp = TimeUtil.GetUnixTimestamp()
            };
            return response;
        }
    }
}