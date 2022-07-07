using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Handlers.UserSession.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Data;

namespace Skateboard3Server.Blaze.Handlers.UserSession;

public class LookupUsersHandler : IRequestHandler<LookupUsersRequest, LookupUsersResponse>
{
    private readonly Skateboard3Context _dbContext;
    private readonly IClientManager _clientManager;

    public LookupUsersHandler(Skateboard3Context dbContext, IClientManager clientManager)
    {
        _dbContext = dbContext;
        _clientManager = clientManager;
    }

    public Task<LookupUsersResponse> Handle(LookupUsersRequest request, CancellationToken cancellationToken)
    {
        var foundUsers = new List<UserInformation>();
        //TODO lookup connected first, then check db?
        if (request.LookupType == UserLookupType.PersonaName)
        {
            foreach (var requestUser in request.Users)
            {
                var users = _dbContext.Personas.Where(x => x.Username == requestUser.Username).Select(x => new UserInformation
                {
                    Id = x.UserId,
                    PersonaId = x.Id,
                    AccountId = x.User.AccountId,
                    AccountLocale = 1701729619, //enUS //TODO dont hardcode
                    Username = x.Username,
                    ExternalId = x.ExternalId,
                    ExternalBlob = x.ExternalBlob,
                }).ToList();
                foundUsers.AddRange(users);
            }
        }

        //check online status
        foreach (var user in foundUsers)
        {
            if (_clientManager.PersonaConnected((uint)user.PersonaId))
            {
                user.Online = true;
            }
        }

        return Task.FromResult(new LookupUsersResponse
        {
            Users = foundUsers
        });
    }
}