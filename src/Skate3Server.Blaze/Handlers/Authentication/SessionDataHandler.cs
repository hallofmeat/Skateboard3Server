using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Authentication.Messages;

namespace Skate3Server.Blaze.Handlers.Authentication
{
    public class SessionDataHandler : IRequestHandler<SessionDataRequest, SessionDataResponse>
    {
        public async Task<SessionDataResponse> Handle(SessionDataRequest request, CancellationToken cancellationToken)
        {
            var response = new SessionDataResponse
            {
                BlazeId = 1234, //TODO
                FirstLogin = false, //TODO
                Key = "12345678_aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", //TODO
                LastLoginTime = 0, //TODO
                Email = "", //nobody@ea.com normally
                Profile = new SessionDataProfile
                {
                    DisplayName = "", //TODO should be session username
                    LastUsed = 0, //TODO
                    ProfileId = 1234, //TODO,
                    ExternalProfileId = 0, //TODO
                    ExternalProfileType = ExternalProfileType.Unknown,
                },
                UserId = 1234, //TODO
            };
            return response;
        }
    }
}