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
                Agup = 0,
                Priv = "",
                Session = new Session
                {
                    BUserId = 1, //TODO
                    Frst = 0, //TODO
                    Key = "", //TODO
                    LastLogin = 0, //TODO
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
                Spam = 0,
                Thst = "",
                Turi = ""
            };
            return response;
        }
    }
}