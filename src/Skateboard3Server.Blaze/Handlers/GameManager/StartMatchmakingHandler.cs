using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Notifications.GameManager;
using Skateboard3Server.Blaze.Notifications.UserSession;
using Skateboard3Server.Blaze.Server;
using Skateboard3Server.Data;

namespace Skateboard3Server.Blaze.Handlers.GameManager;

public class StartMatchmakingHandler : IRequestHandler<StartMatchmakingRequest, StartMatchmakingResponse>
{
    private readonly IBlazeNotificationHandler _notificationHandler;
    private readonly Skateboard3Context _context;
    private readonly BlazeConfig _blazeConfig;
    private readonly IGameManager _gameManager;
    private readonly IUserSessionManager _userSessionManager;
    private readonly ClientContext _clientContext;

    public StartMatchmakingHandler(ClientContext clientContext, Skateboard3Context context, IOptions<BlazeConfig> blazeConfig, IBlazeNotificationHandler notificationHandler, IGameManager gameManager, IUserSessionManager userSessionManager)
    {
        _notificationHandler = notificationHandler;
        _context = context;
        _blazeConfig = blazeConfig.Value;
        _gameManager = gameManager;
        _userSessionManager = userSessionManager;
        _clientContext = clientContext;
    }
    public async Task<StartMatchmakingResponse> Handle(StartMatchmakingRequest request, CancellationToken cancellationToken)
    {
        if (_clientContext.UserSessionId == null)
        {
            throw new Exception("UserSessionId not on context");
        }
        var currentSession = _userSessionManager.GetSession(_clientContext.UserSessionId.Value);

        var firstQosHost = _blazeConfig.QosHosts.FirstOrDefault();
        if (firstQosHost == null)
        {
            throw new Exception("BlazeConfig.QosHosts was not configured");
        }

        //uint matchmakingId = _matchmakingManager.CreateMatchmakingSession();
        uint matchmakingId = 1234;
        var response = new StartMatchmakingResponse
        {
            MatchmakingSessionId = matchmakingId
        };

        //TODO async
        await _notificationHandler.EnqueueNotification(new MatchmakingStatusNotification
        {
            Asil = new List<AsilData>
            {
                new AsilData
                {
                    Cgs = new CgsData
                    {
                        Evst = 0,
                        Mmsn = 0,
                        Nomp = 0
                    },
                    Cust = new CustData
                    {
                        Exps = new ExpsData
                        {
                            Mask = 483 //TODO dlc mask?
                        }
                    },
                    Dnfs = new DnfsData
                    {
                        Mdnf = 0, //TODO
                        Xdnf = 101 //TODO
                    },
                    Fgs = new FgsData
                    {
                        Gnum = 13 //TODO
                    },
                    Geos = new GeosData
                    {
                        Dist = 0
                    },
                    Grda = new Dictionary<string, GrdaData>  //TODO should match attrs
                    {
                        { "difficultyModeRule", new GrdaData
                        {
                            Name = "difficultyModeRule",
                            Values = new List<string> { "1" }
                        }},//TODO: fill in rest here
                    },
                    Gsrd = new GsrdData
                    {
                        Pmax = 6,
                        Pmin = 1 //TODO: should be 2
                    },
                    Hbrd = new HbrdData
                    {
                        Bval = 1 //TODO
                    },
                    Hvrd = new HvrdData
                    {
                        Vval = 1 //TODO
                    },
                    PingServerNames = new PingServerNames
                    {
                        Values = _blazeConfig.QosHosts.Select(x => x.Name).ToList() //qos servers
                    },
                    Rrda = new RrdaData
                    {
                        Rval = 13
                    },
                    Tsrs = new TsrsData(),
                }
            },
            MatchmakingSessionId = matchmakingId,
            UserSessionId = currentSession.SessionId
        });

        //TODO: matchmaking manager?
        var game = _gameManager.FindGame(games => games.FirstOrDefault());
        if (game == null) //No game exists go ahead and force a new one
        {
            await _notificationHandler.EnqueueNotification(new MatchmakingFinishedNotification
            {
                Fit = 0,
                GameId = 0,
                Maxf = 0,
                MatchmakingSessionId = matchmakingId,
                Result = MatchmakingResult.TimedOut, //TimeOut will result in GameManager 0x19 message
                UserSessionId = currentSession.SessionId
            });
            return response;
        }

        //Join existing game
        await _notificationHandler.EnqueueNotification(new MatchmakingFinishedNotification
        {
            Fit = 100, //TODO: what value?
            GameId = game.GameId,
            Maxf = 100, //TODO: what value?
            MatchmakingSessionId = matchmakingId,
            Result = MatchmakingResult.JoinedExistingGame, //TODO: I think this value is correct
            UserSessionId = currentSession.SessionId
        });


        //Tell the new user about all players and the game
        foreach (var player in game.Players)
        {
            if (player == null) continue; //empty slot
            await _notificationHandler.SendNotification(new UserAddedNotification
            {
                AccountId = player.AccountId,
                AccountLocale = 1701729619, //enUS
                ExternalBlob = player.ExternalBlob,
                ExternalId = player.ExternalId,
                Id = player.UserId,
                Username = player.Username,
                Online = true,
                PersonaId = player.PersonaId
            });

            await _notificationHandler.SendNotification(new UserExtendedDataNotification
            {
                UserId = player.UserId,
                Data = new ExtendedData
                {
                    Address = new KeyValuePair<NetworkAddressType, NetworkAddress>(NetworkAddressType.Pair, player.NetworkAddress),
                    PingServerName = firstQosHost.Name,
                    Cty = "",
                    DataMap = new Dictionary<uint, int>(), //TODO: this is missing from the real response
                    HardwareFlags = 0,
                    Pings = new List<int> { 10 }, //TODO: dont harcode
                    NetworkData = new QosNetworkData //TODO: dont hardcode
                    {
                        DownstreamBitsPerSecond = 100000,
                        NatType = NatType.Open,
                        UpstreamBitsPerSecond = 100000
                    },
                    UserAttributes = 0,
                    UserGameList = new List<ulong>{ ((ulong)0x4 << 48 | (ulong)0x1 << 32 | game.GameId) } //gross
                }
            });
        }

        _gameManager.AddPlayer(game.GameId, new Player
        {
            AccountId = currentSession.AccountId,
            UserId = currentSession.UserId,
            PersonaId = currentSession.PersonaId,
            Username = currentSession.Username,
            ExternalId = currentSession.ExternalId,
            ExternalBlob = currentSession.ExternalBlob,
            State = PlayerState.Connecting,
            ConnectTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1000, //microseconds
            NetworkAddress = currentSession.NetworkAddress,
        });

        var currentUserAdded = new UserAddedNotification
        {
            AccountId = currentSession.AccountId,
            AccountLocale = 1701729619, //enUS
            ExternalBlob = currentSession.ExternalBlob,
            ExternalId = currentSession.ExternalId,
            Id = currentSession.UserId,
            Username = currentSession.Username,
            Online = true,
            PersonaId = currentSession.PersonaId
        };

        var currentUserExtended = new UserExtendedDataNotification
        {
            UserId = currentSession.UserId,
            Data = new ExtendedData
            {
                Address = new KeyValuePair<NetworkAddressType, NetworkAddress>(NetworkAddressType.Pair,
                    currentSession.NetworkAddress),
                PingServerName = firstQosHost.Name,
                Cty = "",
                DataMap = new Dictionary<uint, int>(), //TODO: this is missing from the real response
                HardwareFlags = 0,
                Pings = new List<int> {10}, //TODO: dont harcode
                NetworkData = new QosNetworkData //TODO: dont hardcode
                {
                    DownstreamBitsPerSecond = 100000,
                    NatType = NatType.Open,
                    UpstreamBitsPerSecond = 100000
                },
                UserAttributes = 0,
                UserGameList = new List<ulong> {((ulong) 0x4 << 48 | (ulong) 0x1 << 32 | game.GameId)} //gross
            }
        };

        var players = new List<PlayerData>();
        foreach (var player in game.Players)
        {
            if (player == null) continue; //empty slot
            await _notificationHandler.SendNotification(player.UserId, currentUserExtended);
            await _notificationHandler.SendNotification(player.UserId, currentUserAdded);

            players.Add(new PlayerData
            {
                Blob = null,
                ExternalId = player.ExternalId,
                Gid = game.GameId,
                Locale = 1701729619, //enUS
                Username = player.Username,
                NetworkData = new QosNetworkData(),
                PlayerAttributes = new Dictionary<string, string>
                {
                    {"dlc_mask", "483"} //matches start matchmaking MASK?
                },
                PersonaId = player.UserId, //TODO should be PlayerId?
                PlayerNetwork = new KeyValuePair<NetworkAddressType, PairNetworkAddress>(NetworkAddressType.Pair, player.NetworkAddress),
                Sid = player.SlotId,
                Slot = 0,
                PlayerState = player.State,
                Team = 65535,
                Tidx = 65535,
                Time = player.ConnectTime,
                UserId = player.UserId
            });
        }

        //Tell new player about the game
        await _notificationHandler.EnqueueNotification(new GameSetupNotification
        {
            Error = 0,
            GameData = new GameData
            {
                Admins = new List<uint> { game.AdminId },
                Attributes = game.Attributes,
                Cap = new List<ushort> { game.Capacity, 0 },
                GameId = game.GameId,
                GameName = game.Name,
                Gpvh = 1, //TODO hardcoded value?
                GameSettings = game.Settings,
                Gsid = 1, //normally random but not used anywhere
                GameState = game.State,
                Gver = 1,
                HostNetwork = new List<KeyValuePair<NetworkAddressType, PairNetworkAddress>>
                {
                    new KeyValuePair<NetworkAddressType, PairNetworkAddress>(NetworkAddressType.Pair, game.HostNetwork)
                },
                Hses = 1234567, //TODO host session Id?
                Ignore = false,
                Matr = new Dictionary<string, string>
                {
                    {"tprt", "0"},
                    {"vprt", "0"} //TODO? voice port?
                },
                Mcap = game.Capacity, //6 or 30?
                NetworkData = new QosNetworkData(),
                NetworkTopology = NetworkTopology.PeerToPeerFullMesh,
                Pgid = "",
                Pgsr = null,
                PlatformHost = new HostData
                {
                    HostPersonaId = game.HostId,
                    HostSlot = 0
                },
                PingServerName = "",
                QueueCapacity = 0,
                Seed = 12345678, //TODO: random?
                TopologyHost = new HostData
                {
                    HostPersonaId = game.HostId,
                    HostSlot = 0
                },
                Uuid = game.Uuid,
                VoipTopology = VoipTopology.PeerToPeer,
                VersionString = game.Version,
                Xnnc = null,
                Xses = null
            },
            MatchmakingId = matchmakingId, //same as start matchmaking
            Players = players
        });
        return response;
    }
}