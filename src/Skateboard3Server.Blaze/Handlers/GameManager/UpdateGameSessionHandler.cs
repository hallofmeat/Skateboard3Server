using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;

namespace Skateboard3Server.Blaze.Handlers.GameManager;

public class UpdateGameSessionHandler : IRequestHandler<UpdateGameSessionRequest, UpdateGameSessionResponse>
{
    public Task<UpdateGameSessionResponse> Handle(UpdateGameSessionRequest request, CancellationToken cancellationToken)
    {
        var response = new UpdateGameSessionResponse();
        return Task.FromResult(response);
    }
}