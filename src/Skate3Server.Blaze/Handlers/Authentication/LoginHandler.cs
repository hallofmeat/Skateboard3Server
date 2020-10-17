using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Authentication.Messages;
using Skate3Server.Blaze.Notifications.UserSession.Messages;

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

            response.Notifications.Add(new BlazeHeader
            {
                Component = BlazeComponent.UserSession,
                Command = 0x2,
                MessageId = 0,
                MessageType = BlazeMessageType.Notification,
                ErrorCode = 0
            }, new UserAddedNotification
            {
               AccountId = 1234,
               AccountLocale = 1701729619, //enUS
               ExternalBlob = new byte[0],
               Id = 1234,
               ProfileId = 1234,
               Username = "testUser",
               ExternalId = 1234,
               Online = true
            });

            response.Notifications.Add(new BlazeHeader
            {
                Component = BlazeComponent.UserSession,
                Command = 0x1,
                MessageId = 0,
                MessageType = BlazeMessageType.Notification,
                ErrorCode = 0
            }, new UserExtendedDataNotification
            {
                Data = new ExtendedData
                {
                    Address = new KeyValuePair<NetworkAddressType, object>(NetworkAddressType.Unset, ""),
                    Bps = "",
                    Cty = "",
                    Dmap = new Dictionary<uint, int>
                    {
                        { 0x00070047 , 0 }
                    },
                    HardwareFlags = 0,
                    NetworkData = new NetworkData
                    {
                        DownstreamBitsPerSecond = 0,
                        NatType = NatType.Open,
                        UpstreamBitsPerSecond = 0
                    },
                    Uatt = 0
                },
                UserSessionId = 1234
            });

            return response;
        }
    }
}