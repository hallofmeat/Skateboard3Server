using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Authentication.Messages;

namespace Skate3Server.Blaze.Handlers.Redirector
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var response = new LoginResponse
            {
                Agup = false,
                Priv = "",
                Session = new Session
                {
                    BlazeId = 1, //TODO
                    FirstLogin = false, //TODO
                    Key = "", //TODO
                    LastLoginTime = 0, //TODO
                    Email = "",
                    Profile = new Profile
                    {
                        Username = "testUser", //TODO
                        LastUsed = 0, //TODO
                        ProfileId = 1, //TODO,
                        ExternalProfileId = 1234, //TODO
                        ExternalProfileType = ExternalProfileType.PS3,
                    },
                    UserId = 1, //TODO
                },
                Spam = false,
                TermsHost = "",
                TermsUrl = ""
            };
            return response;
        }
    }
}