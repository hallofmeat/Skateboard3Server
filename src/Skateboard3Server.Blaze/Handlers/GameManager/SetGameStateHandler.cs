using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;

namespace Skateboard3Server.Blaze.Handlers.GameManager
{
    public class SetGameStateHandler : IRequestHandler<SetGameStateRequest, SetGameStateResponse>
    {
        public async Task<SetGameStateResponse> Handle(SetGameStateRequest request, CancellationToken cancellationToken)
        {
            var response = new SetGameStateResponse();
            //TODO notify other players game settings changed
            return response;
        }
    }
}