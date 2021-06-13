using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;
using Skateboard3Server.Blaze.Notifications.GameManager;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.GameManager
{
    public class CreateGameHandler : IRequestHandler<CreateGameRequest, CreateGameResponse>
    {
        private readonly IBlazeNotificationHandler _notificationHandler;
        private readonly ClientContext _clientContext;

        public CreateGameHandler(ClientContext clientContext, IBlazeNotificationHandler notificationHandler)
        {
            _notificationHandler = notificationHandler;
            _clientContext = clientContext;
        }

        public async Task<CreateGameResponse> Handle(CreateGameRequest request, CancellationToken cancellationToken)
        {
            if (_clientContext.UserId == null || _clientContext.ExternalId == null)
            {
                throw new Exception("UserId/ExternalId not on context");
            }

            var currentUserId = _clientContext.UserId.Value;
            var currentExternalId = _clientContext.ExternalId.Value;

            uint gameId = 12345; //TODO
            uint gameSessionId = 54321; //TODO right name?
            var response = new CreateGameResponse
            {
                GameId = gameId
            };

            await _notificationHandler.EnqueueNotification(currentUserId, new GameSetupNotification
            {
                Error = 0,
                GameData = new GameData
                {
                    Admins = new List<uint> { currentUserId },
                    Attributes = request.Attributes,
                    Cap = new List<ushort> { 6, 0 },
                    GameId = gameId,
                    GameName = request.GameName,
                    Gpvh = 1, //TODO hardcoded value?
                    GameSettings = request.GameSettings,
                    Gsid = gameSessionId,
                    GameState = GameState.Init,
                    Gver = 1,
                    Hnet = new List<KeyValuePair<NetworkAddressType, PairNetworkAddress>>
                    {
                        new KeyValuePair<NetworkAddressType, PairNetworkAddress>(NetworkAddressType.Pair, //TODO: pair on host, clientip on player
                            new PairNetworkAddress
                            {
                                ExternalIp = new ClientNetworkAddress //TODO return from request make them the host
                                {
                                    Ip = 2130706433, //127.0.0.1
                                    Port = 10000 //TODO
                                },
                                InternalIp = new ClientNetworkAddress
                                {
                                    Ip = 2130706433, //127.0.0.1
                                    Port = 10000 //TODO
                                }
                            })
                    },
                    Hses = 1234567,
                    Ignore = false,
                    Matr = null,
                    //Matr = new Dictionary<string, string> //TODO sent on not host
                    //{
                    //    {"tprt", "0"},
                    //    {"vprt", "9041"} //TODO? voice port?
                    //},
                    Mcap = 6,
                    NetworkData = new QosNetworkData(),
                    NetworkTopology = NetworkTopology.PeerToPeerFullMesh,
                    Pgid = Guid.NewGuid().ToString(),
                    Pgsr = null,
                    Phst = new HstData //Phst vs Thst?
                    {
                        Hpid = currentUserId,
                        Hslt = 0
                    },
                    Psas = "",
                    QueueCapacity = 0,
                    Seed = 12345678, //TODO: random?
                    Thst = new HstData
                    {
                        Hpid = currentUserId,
                        Hslt = 0
                    },
                    Uuid = Guid.NewGuid().ToString(),
                    VoipTopology = VoipTopology.PeerToPeer,
                    VersionString = "Skate3-1",
                    Xnnc = null,
                    Xses = null
                },
                Mmid = 123, //same as start matchmaking
                Pros = new List<PlayerData>
                {
                    new PlayerData
                    {
                        Blob = null,
                        ExternalId = currentExternalId,
                        Gid = gameId,
                        Locale = 1701729619, //enUS
                        Username = _clientContext.Username,
                        NetworkData = new QosNetworkData(),
                        PlayerAttributes = new Dictionary<string, string>
                        {
                            {"dlc_mask", "483"} //matches start matchmaking MASK?
                        },
                        Pid = currentUserId, //TODO should be ProfileId
                        PlayerNetwork = new KeyValuePair<NetworkAddressType, PairNetworkAddress>(
                            NetworkAddressType.Pair, new PairNetworkAddress
                            {
                                ExternalIp = new ClientNetworkAddress
                                {
                                    Ip = 2130706433, //127.0.0.1
                                    Port = 10000
                                },
                                InternalIp = new ClientNetworkAddress
                                {
                                    Ip = 2130706433, //127.0.0.1
                                    Port = 10000
                                },
                            }),
                        Sid = 0,
                        Slot = 0,
                        PlayerState = PlayerState.Connecting,
                        Team = 65535,
                        Tidx = 65535,
                        Time = 0, //DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1000, //microseconds
                        Uid = currentUserId
                    }
                }
            });
            return response;
        }
    }
}