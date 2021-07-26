using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NLog;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Handlers.Authentication.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Notifications.UserSession;
using Skateboard3Server.Blaze.Server;
using Skateboard3Server.Common.Decoders;
using Skateboard3Server.Data;
using Skateboard3Server.Data.Models;

namespace Skateboard3Server.Blaze.Handlers.Authentication
{
    public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
    {
        private readonly Skateboard3Context _context;
        private readonly IBlazeNotificationHandler _notificationHandler;
        private readonly ClientContext _clientContext;
        private readonly IPs3TicketDecoder _ticketDecoder;
        private readonly IUserSessionManager _userSessionManager;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        public LoginHandler(Skateboard3Context context, ClientContext clientContext, IBlazeNotificationHandler notificationHandler, IPs3TicketDecoder ticketDecoder, IUserSessionManager userSessionManager)
        {
            _context = context;
            _clientContext = clientContext;
            _notificationHandler = notificationHandler;
            _ticketDecoder = ticketDecoder;
            _userSessionManager = userSessionManager;
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
                    ExternalIdType = ticket.Body.IssuerId == 100 ? UserExternalIdType.PS3 : UserExternalIdType.Rpcs3, //100 is retail issuerId
                    Username = ticket.Body.Username,
                };
                await _context.Users.AddAsync(user, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                //For now just use the blazeid for both AccountId and PersonaId (this is so the rest of the logic can use those values where they are supposed to)
                user.AccountId = user.Id;
                user.PersonaId = user.Id;
                await _context.SaveChangesAsync(cancellationToken);
            }

            //Create session
            var userSession = _userSessionManager.CreateSession(user.Id);

            //Update login time
            user.LastLogin = TimeUtil.GetUnixTimestamp();
            await _context.SaveChangesAsync(cancellationToken);

            _clientContext.UserId = user.Id;
            _clientContext.UserSessionId = userSession.Id;
            _clientContext.Username = user.Username;
            _clientContext.ExternalId = user.ExternalId;

            var response = new LoginResponse
            {
                Agup = false,
                Priv = "",
                Session = new LoginSession
                {
                    BlazeId = user.Id,
                    FirstLogin = false, //TODO
                    SessionKey = userSession.SessionKey,
                    LastLoginTime = user.LastLogin,
                    Email = "",
                    Persona = new LoginPersona
                    {
                        DisplayName = user.Username,
                        LastUsed = user.LastLogin,
                        PersonaId = user.PersonaId,
                        ExternalId = user.ExternalId,
                        ExternalIdType = ExternalIdType.PS3,
                    },
                    AccountId = user.AccountId,
                },
                Spam = true, //TODO: what is spam?
                TermsHost = "",
                TermsUrl = ""
            };

            await _notificationHandler.EnqueueNotification(user.Id, new UserAddedNotification
            {
                AccountId = user.AccountId,
                AccountLocale = 1701729619, //enUS //TODO: not hardcode
                ExternalBlob = user.ExternalBlob,
                Id = user.Id,
                PersonaId = user.PersonaId,
                Username = user.Username,
                ExternalId = user.ExternalId,
                Online = true
            });

            await _notificationHandler.EnqueueNotification(user.Id, new UserExtendedDataNotification
            {
                Data = new ExtendedData
                {
                    Address = new KeyValuePair<NetworkAddressType, NetworkAddress>(NetworkAddressType.Unset, null),
                    BandwidthServer = "",
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
                    Uatt = 0 //always 0
                },
                UserId = user.Id
            });

            return response;
        }
    }
}