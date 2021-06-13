using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Skateboard3Server.Blaze.Handlers.Authentication.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Server;
using Skateboard3Server.Data;

namespace Skateboard3Server.Blaze.Handlers.Authentication
{
    public class SessionDataHandler : IRequestHandler<SessionDataRequest, SessionDataResponse>
    {
        private readonly BlazeContext _blazeContext;
        private readonly ClientContext _clientContext;
        private readonly IUserSessionManager _userSessionManager;

        public SessionDataHandler(BlazeContext blazeContext, ClientContext clientContext, IUserSessionManager userSessionManager)
        {
            _blazeContext = blazeContext;
            _clientContext = clientContext;
            _userSessionManager = userSessionManager;
        }

        public async Task<SessionDataResponse> Handle(SessionDataRequest request, CancellationToken cancellationToken)
        {
            if (_clientContext.UserId == null || _clientContext.UserSessionId == null)
            {
                throw new Exception("UserId/UserSessionId not on context");
            }
            var currentUserId = _clientContext.UserId.Value;
            var currentSessionId = _clientContext.UserSessionId.Value;

            var user = await _blazeContext.Users.SingleOrDefaultAsync(x => x.Id == currentUserId, cancellationToken);

            if (user == null)
            {
                return null;
            }

            var userSessionKey = _userSessionManager.GetSessionKey(currentSessionId);

            var response = new SessionDataResponse
            {
                BlazeId = user.Id,
                FirstLogin = false,
                SessionKey = userSessionKey,
                LastLoginTime = 0, 
                Email = "", //nobody@ea.com normally
                Profile = new SessionDataProfile
                {
                    DisplayName = user.Username,
                    LastUsed = 0,
                    ProfileId = user.ProfileId,
                    ExternalProfileId = 0,
                    ExternalProfileType = ExternalProfileType.Unknown,
                },
                AccountId = user.AccountId,
            };
            return response;
            
        }
    }
}