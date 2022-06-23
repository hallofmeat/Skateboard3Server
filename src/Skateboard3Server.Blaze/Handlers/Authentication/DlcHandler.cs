using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.Authentication.Messages;

namespace Skateboard3Server.Blaze.Handlers.Authentication;

public class DlcHandler : IRequestHandler<DlcRequest, DlcResponse>
{
    public async Task<DlcResponse> Handle(DlcRequest request, CancellationToken cancellationToken)
    {
        var response = new DlcResponse();
        return response;
    }
}