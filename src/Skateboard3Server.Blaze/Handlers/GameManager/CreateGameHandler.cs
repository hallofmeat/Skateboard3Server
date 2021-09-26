using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;
using Skateboard3Server.Blaze.Managers;
using Skateboard3Server.Blaze.Notifications.GameManager;
using Skateboard3Server.Blaze.Server;
using GameData = Skateboard3Server.Blaze.Notifications.GameManager.GameData;

namespace Skateboard3Server.Blaze.Handlers.GameManager
{
    public class CreateGameHandler : IRequestHandler<CreateGameRequest, CreateGameResponse>
    {
        private readonly IBlazeNotificationHandler _notificationHandler;
        private readonly ClientContext _clientContext;
        private readonly IGameManager _gameManager;
        private readonly IUserSessionManager _userSessionManager;

        public CreateGameHandler(ClientContext clientContext, IBlazeNotificationHandler notificationHandler, IGameManager gameManager, IUserSessionManager userSessionManager)
        {
            _notificationHandler = notificationHandler;
            _clientContext = clientContext;
            _gameManager = gameManager;
            _userSessionManager = userSessionManager;
        }

        public async Task<CreateGameResponse> Handle(CreateGameRequest request, CancellationToken cancellationToken)
        {
            if (_clientContext.UserId == null || _clientContext.UserSessionId == null ||  _clientContext.ExternalId == null)
            {
                throw new Exception("UserId/UserSessionId/ExternalId not on context");
            }

            var currentUserId = _clientContext.UserId.Value;
            var currentExternalId = _clientContext.ExternalId.Value;
            var currentSessionId = _clientContext.UserSessionId.Value;

            var session = _userSessionManager.GetSession(currentSessionId);

            ushort capacity = 6; //TODO: hardcoded capacity
            var game = _gameManager.CreateGame(capacity); //TODO pass game instead of returning it?
            game.Name = request.GameName;
            game.Settings = request.GameSettings;
            game.Attributes = request.Attributes;
            game.Version = request.VersionString;
            game.AdminId = currentUserId; //TODO: make sure this is the right id
            game.HostId = currentUserId; //TODO: make sure this is the right id
            game.HostNetwork = session.NetworkAddress;

            _gameManager.AddPlayer(game.GameId, new Player
            {
                UserId = currentUserId,
                Username = _clientContext.Username,
                ExternalId = currentExternalId,
                State = PlayerState.Connected,
                ConnectTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1000, //microseconds
                NetworkAddress = session.NetworkAddress
            });

            var response = new CreateGameResponse
            {
                GameId = game.GameId
            };

            var players = new List<PlayerData>();
            foreach (var player in game.Players)
            {
                if (player == null) continue; //empty slot
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
                    Uid = player.UserId
                });
            }

            await _notificationHandler.EnqueueNotification(currentUserId, new GameSetupNotification
            {
                Error = 0,
                GameData = new GameData
                {
                    Admins = new List<uint> { game.AdminId },
                    Attributes = game.Attributes,
                    Cap = new List<ushort> { capacity, 0 },
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
                    Hses = 1234567, //TODO
                    Ignore = false,
                    Matr = null,
                    //Matr = new Dictionary<string, string> //TODO sent on not host
                    //{
                    //    {"tprt", "0"},
                    //    {"vprt", "9041"} //TODO? voice port?
                    //},
                    Mcap = capacity, //TODO: also can be 30?
                    NetworkData = new QosNetworkData(),
                    NetworkTopology = NetworkTopology.PeerToPeerFullMesh,
                    Pgid = Guid.NewGuid().ToString(),
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
                MatchmakingId = 1234, //TODO: pull from matchmakingmanager //same as start matchmaking
                Players = players
            });
            return response;
        }
    }
}