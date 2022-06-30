using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NLog;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Handlers.Authentication.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Managers.Models;
using Skateboard3Server.Blaze.Notifications.UserSession;
using Skateboard3Server.Blaze.Server;
using Skateboard3Server.Blaze.Util;
using Skateboard3Server.Common.Decoders;
using Skateboard3Server.Data;
using Skateboard3Server.Data.Models;

namespace Skateboard3Server.Blaze.Handlers.Authentication;

public class LoginHandler : IRequestHandler<LoginRequest, LoginResponse>
{
    private readonly Skateboard3Context _dbContext;
    private readonly IBlazeNotificationHandler _notificationHandler;
    private readonly ClientContext _clientContext;
    private readonly IPs3TicketDecoder _ticketDecoder;
    private readonly IUserSessionManager _userSessionManager;
    private readonly IClientManager _clientManager;

    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
    public LoginHandler(Skateboard3Context dbContext, ClientContext clientContext, IBlazeNotificationHandler notificationHandler, IPs3TicketDecoder ticketDecoder, IUserSessionManager userSessionManager, IClientManager clientManager)
    {
        _dbContext = dbContext;
        _clientContext = clientContext;
        _notificationHandler = notificationHandler;
        _ticketDecoder = ticketDecoder;
        _userSessionManager = userSessionManager;
        _clientManager = clientManager;
    }

    public async Task<LoginResponse> Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        Logger.Debug($"LOGIN for {_clientContext.ConnectionId}");

        var ticket = _ticketDecoder.DecodeTicket(request.Ticket);
        if (ticket == null)
        {
            throw new Exception("Could not parse ticket, unable to login!");
        }

        var persona = await _dbContext.Personas.Include(x => x.User).SingleOrDefaultAsync(x => x.ExternalId == ticket.Body.UserId, cancellationToken: cancellationToken);
            
        //First time we have seen this persona
        if (persona == null)
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

            //Create new user
            var user = new User();
            await _dbContext.Users.AddAsync(user, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            //For now just use the blazeid for both AccountId (this is so the rest of the logic can use those values where they are supposed to)
            user.AccountId = user.Id;

            persona = new Persona
            {
                ExternalId = ticket.Body.UserId,
                ExternalBlob = externalBlob.ToArray(),
                ExternalIdType = ticket.Body.IssuerId == 100 ? PersonaExternalIdType.Ps3 : PersonaExternalIdType.Rpcs3, //100 is retail issuerId
                Username = ticket.Body.Username,
                User = user
            };
            await _dbContext.Personas.AddAsync(persona, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        else
        {
            //check they are already connected
            if (_clientManager.PersonaConnected(persona.Id))
            {
                throw new Exception($"Persona {persona.Id} is already connected, bailing!");
            }
        }

        //Create session
        var newSession = new UserSessionData
        {
            AccountId = persona.User.AccountId,
            UserId = persona.UserId,
            PersonaId = persona.Id,
            Username = persona.Username,
            ExternalId = persona.ExternalId,
            ExternalBlob = persona.ExternalBlob,
        };
        var sessionData = _userSessionManager.StoreSession(newSession);

        //Update login time
        var currentTimestamp = TimeUtil.GetUnixTimestamp();
        persona.LastUsed = currentTimestamp;
        persona.User.LastLogin = currentTimestamp;
        await _dbContext.SaveChangesAsync(cancellationToken);

        _clientContext.UserId = newSession.UserId;
        _clientContext.PersonaId = newSession.PersonaId;
        _clientContext.UserSessionId = sessionData.SessionId;

        var response = new LoginResponse
        {
            Agup = false,
            Priv = "",
            Session = new LoginSession
            {
                BlazeId = newSession.UserId,
                FirstLogin = false, //TODO
                SessionKey = sessionData.SessionKey,
                LastLoginTime = currentTimestamp,
                Email = "",
                Persona = new LoginPersona
                {
                    DisplayName = newSession.Username,
                    LastUsed = currentTimestamp,
                    PersonaId = newSession.UserId,
                    ExternalId = newSession.ExternalId,
                    ExternalIdType = ExternalIdType.Ps3,
                },
                AccountId = newSession.AccountId,
            },
            Spam = true, //TODO: what is spam?
            TermsHost = "",
            TermsUrl = ""
        };

        await _notificationHandler.EnqueueNotification(new UserAddedNotification
        {
            AccountId = newSession.AccountId,
            AccountLocale = 1701729619, //enUS //TODO: not hardcode
            ExternalBlob = newSession.ExternalBlob,
            Id = newSession.UserId,
            PersonaId = newSession.PersonaId,
            Username = newSession.Username,
            ExternalId = newSession.ExternalId,
            Online = true
        });

        await _notificationHandler.EnqueueNotification(new UserExtendedDataNotification
        {
            Data = new ExtendedData
            {
                Address = new KeyValuePair<NetworkAddressType, NetworkAddress?>(NetworkAddressType.Unset, null),
                PingServerName = "",
                Cty = "",
                DataMap = new Dictionary<uint, int>
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
                UserAttributes = 0 //always 0
            },
            UserId = newSession.UserId
        });

        return response;
    }
}