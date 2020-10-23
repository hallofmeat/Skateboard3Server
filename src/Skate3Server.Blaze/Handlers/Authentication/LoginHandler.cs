using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Handlers.Authentication.Messages;
using Skate3Server.Blaze.Notifications.UserSession.Messages;
using Skate3Server.Common.Decoders;

namespace Skate3Server.Blaze.Handlers.Authentication
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly IPs3TicketDecoder _ticketDecoder;

        public LoginHandler(IPs3TicketDecoder ticketDecoder)
        {
            _ticketDecoder = ticketDecoder;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var ticket = _ticketDecoder.DecodeTicket(request.Ticket);
            if (ticket == null)
            {
                throw new Exception("Could not parse ticket, unable to login");
            }

            var response = new LoginResponse
            {
                Agup = false,
                Priv = "",
                Session = new LoginSession
                {
                    BlazeId = 1234, //TODO
                    FirstLogin = false, //TODO
                    BlazeKey = "12345678_aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", //TODO
                    LastLoginTime = 1602965309, //TODO
                    Email = "",
                    Profile = new LoginProfile
                    {
                        DisplayName = ticket.Body.Username, //TODO
                        LastUsed = 1602965280, //TODO
                        ProfileId = 1234, //TODO,
                        ExternalProfileId = ticket.Body.UserId, //TODO
                        ExternalProfileType = ExternalProfileType.PS3,
                    },
                    UserId = 1234, //TODO
                },
                Spam = true,
                TermsHost = "",
                TermsUrl = ""
            };

            //TODO: this is a hack
            var externalBlob = new List<byte>();
            externalBlob.AddRange(Encoding.ASCII.GetBytes(ticket.Body.Username.PadRight(20, '\0')));
            externalBlob.AddRange(Encoding.ASCII.GetBytes(ticket.Body.Domain));
            externalBlob.AddRange(Encoding.ASCII.GetBytes(ticket.Body.Region));
            externalBlob.AddRange(Encoding.ASCII.GetBytes("ps3"));
            externalBlob.Add(0x0);
            externalBlob.Add(0x1);
            externalBlob.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

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
                ExternalBlob = externalBlob.ToArray(),
                Id = 1234,
                ProfileId = 1234,
                Username = ticket.Body.Username,
                ExternalId = ticket.Body.UserId,
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
                    Address = new KeyValuePair<NetworkAddressType, string>(NetworkAddressType.Unset, null),
                    Bps = "",
                    Cty = "",
                    Dmap = new Dictionary<uint, int>
                    {
                        { 0x00070047 , 0 }
                    },
                    HardwareFlags = 0,
                    NetworkData = new QosNetworkData
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