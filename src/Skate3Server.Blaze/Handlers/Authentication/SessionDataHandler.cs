using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Skate3Server.Blaze.Handlers.Authentication.Messages;
using Skate3Server.Blaze.Server;
using Skate3Server.Data;

namespace Skate3Server.Blaze.Handlers.Authentication
{
    public class SessionDataHandler : IRequestHandler<SessionDataRequest, SessionDataResponse>
    {
        private readonly BlazeContext _blazeContext;
        private readonly ClientContext _clientContext;

        public SessionDataHandler(BlazeContext blazeContext, ClientContext clientContext)
        {
            _blazeContext = blazeContext;
            _clientContext = clientContext;
        }

        public async Task<SessionDataResponse> Handle(SessionDataRequest request, CancellationToken cancellationToken)
        {

            var user = await _blazeContext.Users.SingleOrDefaultAsync(x => x.Id == _clientContext.UserId, cancellationToken);

            if (user == null)
            {
                return null;
            }

            var response = new SessionDataResponse
            {
                BlazeId = user.Id,
                FirstLogin = false,
                Key = "12345678_aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", //TODO
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