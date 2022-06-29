using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Handlers.Authentication.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Authentication;

public class SessionDataHandler : IRequestHandler<SessionDataRequest, SessionDataResponse>
{
    private readonly ClientContext _clientContext;
    private readonly IUserSessionManager _userSessionManager;

    public SessionDataHandler(ClientContext clientContext, IUserSessionManager userSessionManager)
    {
        _clientContext = clientContext;
        _userSessionManager = userSessionManager;
    }

    public Task<SessionDataResponse> Handle(SessionDataRequest request, CancellationToken cancellationToken)
    {
        if (_clientContext.UserSessionId == null)
        {
            throw new Exception("UserSessionId not on context");
        }
        var session = _userSessionManager.GetSession(_clientContext.UserSessionId.Value);

        var response = new SessionDataResponse
        {
            BlazeId = session.UserId,
            FirstLogin = false,
            SessionKey = session.SessionKey,
            LastLoginTime = 0, 
            Email = "", //nobody@ea.com normally
            Persona = new SessionDataPersona
            {
                DisplayName = session.Username,
                LastUsed = 0,
                PersonaId = session.PersonaId,
                ExternalId = 0,
                ExternalIdType = ExternalIdType.Unknown,
            },
            AccountId = session.AccountId,
        };
        return Task.FromResult(response);
            
    }
}