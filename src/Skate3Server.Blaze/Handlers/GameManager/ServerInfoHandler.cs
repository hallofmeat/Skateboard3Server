using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Skate3Server.Blaze.Handlers.GameManager.Messages;

namespace Skate3Server.Blaze.Handlers.GameManager
{
    public class StartMachmakingHandler : IRequestHandler<StartMatchmakingRequest, StartMatchmakingResponse>
    {
        private readonly BlazeConfig _blazeConfig;

        public StartMachmakingHandler(IOptions<BlazeConfig> blazeConfig)
        {
            _blazeConfig = blazeConfig.Value;
        }
        public async Task<StartMatchmakingResponse> Handle(StartMatchmakingRequest request, CancellationToken cancellationToken)
        {
            var response = new StartMatchmakingResponse
            {
                Msid = 1234 //TODO
            };
            return response;
        }
    }
}