using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.UserSession.Messages;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.UserSession;

public class LookupUsersHandler : IRequestHandler<LookupUsersRequest, LookupUsersResponse>
{
    private readonly ClientContext _clientContext;

    public LookupUsersHandler(ClientContext clientContext)
    {
        _clientContext = clientContext;
    }

    public async Task<LookupUsersResponse> Handle(LookupUsersRequest request, CancellationToken cancellationToken)
    {
        //TODO: figure out response
        var response = new LookupUsersResponse();
        return response;
    }
}