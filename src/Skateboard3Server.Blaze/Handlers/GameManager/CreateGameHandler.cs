using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Handlers.GameManager.Messages;
using Skateboard3Server.Blaze.Notifications.GameManager;
using Skateboard3Server.Blaze.Notifications.UserSession;
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
            uint gameId = 12345; //TODO
            var response = new CreateGameResponse
            {
                GameId = gameId
            };

            await _notificationHandler.SendNotification(_clientContext.UserId, new UserExtendedDataNotification
            {
                Data = new ExtendedData
                {
                    Address = new KeyValuePair<NetworkAddressType, NetworkAddress>(NetworkAddressType.Pair, new PairNetworkAddress
                    {
                        ExternalIp = new ClientNetworkAddress
                        {
                            Ip = 2130706433,
                            Port = 10000
                        },
                        InternalIp = new ClientNetworkAddress
                        {
                            Ip = 2130706433,
                            Port = 10000
                        },
                    }),
                    Bps = "tst",
                    Cty = "",
                    Dmap = new Dictionary<uint, int>
                    {
                        { 0x00070047 , 0 }
                    },
                    HardwareFlags = 0,
                    Pslm = new List<int> //maps to NLMP
                    {
                        100,
                    },
                    NetworkData = new QosNetworkData
                    {
                        DownstreamBitsPerSecond = 100,
                        NatType = NatType.Open,
                        UpstreamBitsPerSecond = 100
                    },
                    Uatt = 0,
                    Ulst = new List<ulong>
                    {
                        1 //TODO: figure out what this is
                    }
                },
                UserId = _clientContext.UserId
            });

            await _notificationHandler.EnqueueNotification(_clientContext.UserId, new GameSetupNotification
            {
                Error = 0,
                GameData = new GameData
                {
                    Admins = new List<uint> { _clientContext.UserId },
                    Attributes = request.Attributes,
                    Cap = new List<ushort> { 6, 0 },
                    GameId = gameId,
                    GameName = request.GameName,
                    Gpvh = 1000000000,
                    GameSettings = request.GameSettings,
                    Gsid = gameId,
                    GameState = GameState.Init,
                    Gver = 1,
                    Hnet = new List<KeyValuePair<NetworkAddressType, ClientNetworkAddress>>
                    {
                        new KeyValuePair<NetworkAddressType, ClientNetworkAddress>(NetworkAddressType.IpAddress,
                            new ClientNetworkAddress
                            {
                                Ip = 2130706433,
                                Port = 10000
                            })
                    },
                    Hses = 1234567,
                    Igno = true,
                    Matr = new Dictionary<string, string>
                    {
                        {"tprt", "0"},
                        {"vprt", "11030"}
                    },
                    Mcap = 30,
                    NetworkData = new QosNetworkData(),
                    Ntop = 132,
                    Pgid = "",
                    Pgsr = null,
                    Phst = new HstData
                    {
                        Hpid = _clientContext.UserId,
                        Hslt = 1
                    },
                    Psas = "tst",
                    Qcap = 0,
                    Seed = 09877,
                    Thst = new HstData
                    {
                        Hpid = _clientContext.UserId,
                        Hslt = 0
                    },
                    Uuid = Guid.NewGuid().ToString(),
                    Voip = 1,
                    VersionString = "Skate3-1",
                    Xnnc = null,
                    Xses = null
                },
                Mmid = 0,
                Pros = new List<PlayerData>
                {
                    new PlayerData
                    {
                        Blob = null,
                        ExternalId = _clientContext.ExternalId,
                        Gid = gameId,
                        Locale = 1701729619, //enUS
                        Username = _clientContext.Username,
                        NetworkData = new QosNetworkData(),
                        PlayerAttributes = new Dictionary<string, string>
                        {
                            {"dlc_mask", "1"}
                        },
                        Pid = _clientContext.UserId, //TODO should be ProfileId
                        PlayerNetwork = new KeyValuePair<NetworkAddressType, PairNetworkAddress>(
                            NetworkAddressType.Pair, new PairNetworkAddress
                            {
                                ExternalIp = new ClientNetworkAddress
                                {
                                    Ip = 2130706433,
                                    Port = 10000
                                },
                                InternalIp = new ClientNetworkAddress
                                {
                                    Ip = 2130706433,
                                    Port = 10000
                                },
                            }),
                        Sid = 1,
                        Slot = 0,
                        Stat = 4,
                        Team = 65535,
                        Tidx = 65535,
                        Time = 10000000,
                        Uid = _clientContext.UserId
                    }
                }
            });
            return response;
        }
    }
}