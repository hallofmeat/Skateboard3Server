using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NLog;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Handlers.Authentication.Messages;
using Skate3Server.Blaze.Notifications.UserSession;
using Skate3Server.Blaze.Server;
using Skate3Server.Common.Decoders;
using Skate3Server.Data;
using Skate3Server.Data.Models;

namespace Skate3Server.Blaze.Handlers.Authentication
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly BlazeContext _context;
        private readonly ClientContext _clientContext;
        private readonly IPs3TicketDecoder _ticketDecoder;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public LoginHandler(BlazeContext context, ClientContext clientContext, IPs3TicketDecoder ticketDecoder)
        {
            _context = context;
            _clientContext = clientContext;
            _ticketDecoder = ticketDecoder;
        }

        public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            Logger.Debug($"LOGIN for {_clientContext.ConnectionId}");

            var ticket = _ticketDecoder.DecodeTicket(request.Ticket);
            if (ticket == null)
            {
                throw new Exception("Could not parse ticket, unable to login");
            }

            var user = await _context.Users.SingleOrDefaultAsync(x => x.ExternalId == ticket.Body.UserId, cancellationToken: cancellationToken);

            //First time we have seen this user
            if (user == null)
            {
                //TODO: a hack, this normally comes from the auth new login flow but I dont want to prompt for a login
                var externalBlob = new List<byte>();
                externalBlob.AddRange(Encoding.ASCII.GetBytes(ticket.Body.Username.PadRight(20, '\0')));
                externalBlob.AddRange(Encoding.ASCII.GetBytes(ticket.Body.Domain));
                externalBlob.AddRange(Encoding.ASCII.GetBytes(ticket.Body.Region));
                externalBlob.AddRange(Encoding.ASCII.GetBytes("ps3"));
                externalBlob.Add(0x0);
                externalBlob.Add(0x1);
                externalBlob.AddRange(new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });

                user = new User
                {
                    ExternalId = ticket.Body.UserId,
                    ExternalBlob = externalBlob.ToArray(),
                    ExternalIdType = UserExternalIdType.PS3,
                    Username = ticket.Body.Username,
                };
                await _context.Users.AddAsync(user, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                //For now just use the blazeid for both AccountId and ProfileId (this is so the rest of the logic can use those values where they are supposed to)
                user.AccountId = user.Id;
                user.ProfileId = user.Id;
                await _context.SaveChangesAsync(cancellationToken);
            }

            var response = new LoginResponse
            {
                Agup = false,
                Priv = "",
                Session = new LoginSession
                {
                    BlazeId = user.Id,
                    FirstLogin = false, //TODO
                    BlazeKey = "12345678_aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", //TODO
                    LastLoginTime = user.LastLogin,
                    Email = "",
                    Profile = new LoginProfile
                    {
                        DisplayName = user.Username,
                        LastUsed = user.LastLogin,
                        ProfileId = user.ProfileId,
                        ExternalProfileId = user.ExternalId,
                        ExternalProfileType = ExternalProfileType.PS3,
                    },
                    AccountId = user.AccountId,
                },
                Spam = true, //TODO: what is spam?
                TermsHost = "",
                TermsUrl = ""
            };

            _clientContext.Notifications.Enqueue((new BlazeHeader
            {
                Component = BlazeComponent.UserSession,
                Command = (ushort)UserSessionNotification.UserAdded,
                MessageId = 0,
                MessageType = BlazeMessageType.Notification,
                ErrorCode = 0
            }, new UserAddedNotification
            {
                AccountId = user.AccountId,
                AccountLocale = 1701729619, //enUS //TODO: not hardcode
                ExternalBlob = user.ExternalBlob,
                Id = user.Id,
                ProfileId = user.ProfileId,
                Username = user.Username,
                ExternalId = user.ExternalId,
                Online = true
            }));

            _clientContext.Notifications.Enqueue((new BlazeHeader
            {
                Component = BlazeComponent.UserSession,
                Command = (ushort)UserSessionNotification.UserExtendedData,
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
                UserId = user.Id
            }));

            //Update login time
            user.LastLogin = TimeUtil.GetUnixTimestamp();
            await _context.SaveChangesAsync(cancellationToken);

            _clientContext.UserId = user.Id;
            _clientContext.Username = user.Username;

            return response;
        }
    }
}