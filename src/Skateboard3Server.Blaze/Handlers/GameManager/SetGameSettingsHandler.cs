using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;

namespace Skateboard3Server.Blaze.Handlers.GameManager
{
    public class SetGameSettingsHandler : IRequestHandler<SetGameSettingsRequest, SetGameSettingsResponse>
    {
        public async Task<SetGameSettingsResponse> Handle(SetGameSettingsRequest request, CancellationToken cancellationToken)
        {
            var response = new SetGameSettingsResponse();
            //TODO notify other players game settings changed
            return response;
        }
    }
}