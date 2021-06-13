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
    public class StartMachmakingHandler : IRequestHandler<StartMatchmakingRequest, StartMatchmakingResponse>
    {
        private readonly IBlazeNotificationHandler _notificationHandler;
        private readonly ClientContext _clientContext;

        public StartMachmakingHandler(ClientContext clientContext, IBlazeNotificationHandler notificationHandler)
        {
            _notificationHandler = notificationHandler;
            _clientContext = clientContext;
        }
        public async Task<StartMatchmakingResponse> Handle(StartMatchmakingRequest request, CancellationToken cancellationToken)
        {
            if (_clientContext.UserId == null || _clientContext.UserSessionId == null)
            {
                throw new Exception("UserId/UserSessionId not on context");
            }
            var currentUserId = _clientContext.UserId.Value;
            var currentSessionId = _clientContext.UserSessionId.Value;

            uint gameId = 12345; //TODO
            uint matchmakingId = 123; //TODO
            var response = new StartMatchmakingResponse
            {
                MatchmakingSessionId = matchmakingId //TODO
            };

            await _notificationHandler.EnqueueNotification(currentUserId, new MatchmakingStatusNotification
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
                                Mask = 483 //TODO
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
                        Psrs = new PsrsData
                        {
                            Values = new List<string> { "tst" } //qos servers
                        },
                        Rrda = new RrdaData
                        {
                            Rval = 13
                        },
                        Tsrs = new TsrsData(),
                    }
                },
                MatchmakingSessionId = matchmakingId,
                UserSessionId = currentSessionId
            });

            await _notificationHandler.EnqueueNotification(currentUserId, new MatchmakingFinishedNotification
            {
                Fit = 0,
                GameId = 0,
                Maxf = 0,
                MatchmakingSessionId = matchmakingId,
                Result = MatchmakingResult.TimedOut, //TimeOut will result in GameManager 0x19 message
                UserSessionId = currentSessionId
            });


            //await _notificationHandler.SendNotification(_clientContext.UserId, new UserAddedNotification
            //{
            //    AccountId = 99999,
            //    AccountLocale = 1701729619, //enUS
            //    ExternalBlob = null,
            //    ExternalId = 0,
            //    Id = 99999,
            //    Username = "@test",
            //    Online = true,
            //    ProfileId = 99999
            //});

            //await _notificationHandler.SendNotification(_clientContext.UserId, new UserExtendedDataNotification
            //{
            //    UserId = 99999,
            //    Data = new ExtendedData
            //    {
            //        Address = new KeyValuePair<NetworkAddressType, NetworkAddress>(NetworkAddressType.Unset, null),
            //        Bps = "",
            //        Cty = "",
            //        Dmap = new Dictionary<uint, int>(), //TODO: this is missing from the real response
            //        HardwareFlags = 0,
            //        NetworkData = new QosNetworkData
            //        {
            //            DownstreamBitsPerSecond = 0,
            //            NatType = NatType.Unknown,
            //            UpstreamBitsPerSecond = 0
            //        },
            //        Uatt = 0,
            //    }
            //});

            //await _notificationHandler.EnqueueNotification(_clientContext.UserId, new GameSetupNotification
            //{
            //    Error = 0,
            //    GameData = new GameData
            //    {
            //        Admins = new List<uint> {_clientContext.UserId},
            //        Attributes = request.Attributes,
            //        Cap = new List<ushort> {6, 0},
            //        GameId = gameId,
            //        Gnam = request.Gnam,
            //        Gpvh = 1000000000,
            //        Gset = request.Gset,
            //        Gsid = gameId,
            //        GameState = GameState.Init,
            //        Gver = 1,
            //        Hnet = new List<KeyValuePair<NetworkAddressType, ClientNetworkAddress>>
            //        {
            //            new KeyValuePair<NetworkAddressType, ClientNetworkAddress>(NetworkAddressType.IpAddress,
            //                new ClientNetworkAddress
            //                {
            //                    Ip = 2130706433,
            //                    Port = 10000
            //                })
            //        },
            //        Hses = 1234567,
            //        Igno = true,
            //        Matr = new Dictionary<string, string>
            //        {
            //            {"tprt", "0"},
            //            {"vprt", "11030"}
            //        },
            //        Mcap = 30,
            //        NetworkData = new QosNetworkData(),
            //        Ntop = 132,
            //        Pgid = "",
            //        Pgsr = null,
            //        Phst = new HstData
            //        {
            //            Hpid = _clientContext.UserId,
            //            Hslt = 1
            //        },
            //        Psas = "tst",
            //        Qcap = 0,
            //        Seed = 09877,
            //        Thst = new HstData
            //        {
            //            Hpid = _clientContext.UserId,
            //            Hslt = 0
            //        },
            //        Uuid = Guid.NewGuid().ToString(),
            //        Voip = 1,
            //        VersionString = "Skate3-1",
            //        Xnnc = null,
            //        Xses = null
            //    },
            //    Mmid = 0,
            //    Pros = new List<PlayerData>
            //    {
            //        new PlayerData
            //        {
            //            Blob = null,
            //            ExternalId = _clientContext.ExternalId,
            //            Gid = gameId,
            //            Locale = 1701729619, //enUS
            //            Username = _clientContext.Username,
            //            NetworkData = new QosNetworkData(),
            //            PlayerAttributes = new Dictionary<string, string>
            //            {
            //                {"dlc_mask", "1"}
            //            },
            //            Pid = _clientContext.UserId, //TODO should be ProfileId
            //            PlayerNetwork = new KeyValuePair<NetworkAddressType, PairNetworkAddress>(
            //                NetworkAddressType.Pair, new PairNetworkAddress
            //                {
            //                    ExternalIp = new ClientNetworkAddress
            //                    {
            //                        Ip = 2130706433,
            //                        Port = 10000
            //                    },
            //                    InternalIp = new ClientNetworkAddress
            //                    {
            //                        Ip = 2130706433,
            //                        Port = 10000
            //                    },
            //                }),
            //            Sid = 1,
            //            Slot = 0,
            //            Stat = 4,
            //            Team = 65535,
            //            Tidx = 65535,
            //            Time = 10000000,
            //            Uid = _clientContext.UserId
            //        }
            //    }
            //});

            //await _notificationHandler.SendNotification(_clientContext.UserId, new UserExtendedDataNotification
            //{
            //    Data = new ExtendedData
            //    {
            //        Address = new KeyValuePair<NetworkAddressType, NetworkAddress>(NetworkAddressType.Pair, new PairNetworkAddress
            //        {
            //            ExternalIp = new ClientNetworkAddress
            //            {
            //                Ip = 2130706433,
            //                Port = 10000
            //            },
            //            InternalIp = new ClientNetworkAddress
            //            {
            //                Ip = 2130706433,
            //                Port = 10000
            //            },
            //        }),
            //        Bps = "tst",
            //        Cty = "",
            //        Dmap = new Dictionary<uint, int>
            //        {
            //            { 0x00070047 , 0 }
            //        },
            //        HardwareFlags = 0,
            //        Pslm = new List<int> //maps to NLMP
            //        {
            //            100,
            //        },
            //        NetworkData = new QosNetworkData
            //        {
            //            DownstreamBitsPerSecond = 100,
            //            NatType = NatType.Open,
            //            UpstreamBitsPerSecond = 100
            //        },
            //        Uatt = 0,
            //        Ulst = new List<ulong>
            //        {
            //            1 //TODO: figure out what this is
            //        }
            //    },
            //    UserId = _clientContext.UserId
            //});




            //await _notificationHandler.EnqueueNotification(_clientContext.UserId, new MatchmakingFinishedNotification
            //{
            //    Fit = 100,
            //    GameId = gameId,
            //    Maxf = 100,
            //    Msid = msid,
            //    Result = MatchmakingResult.CreatedGame,
            //    Usid = _clientContext.UserId
            //});
            //TODO: we are just creating a game with just the user in it


            //await _notificationHandler.EnqueueNotification(_clientContext.UserId, new GameSetupNotification
            //{
            //    Error = 0,
            //    GameData = new GameData
            //    {
            //        Admins = new List<uint> { _clientContext.UserId },
            //        Attributes = request.Attributes,
            //        Cap = new List<ushort> { 6, 0 },
            //        GameId = gameId,
            //        Gnam = request.Gnam,
            //        Gpvh = 1000000000,
            //        Gset = request.Gset,
            //        Gsid = gameId,
            //        GameState = GameState.Init,
            //        Gver = 1,
            //        Hnet = new List<KeyValuePair<NetworkAddressType, ClientNetworkAddress>>
            //        {
            //            new KeyValuePair<NetworkAddressType, ClientNetworkAddress>(NetworkAddressType.IpAddress,
            //                new ClientNetworkAddress
            //                {
            //                    Ip = 2130706433,
            //                    Port = 10000
            //                })
            //        },
            //        Hses = 1234567,
            //        Igno = true,
            //        Matr = new Dictionary<string, string>
            //        {
            //            {"tprt", "0"},
            //            {"vprt", "11030"}
            //        },
            //        Mcap = 30,
            //        NetworkData = new QosNetworkData(),
            //        Ntop = 132,
            //        Pgid = "",
            //        Pgsr = null,
            //        Phst = new HstData
            //        {
            //            Hpid = _clientContext.UserId,
            //            Hslt = 1
            //        },
            //        Psas = "tst",
            //        Qcap = 0,
            //        Seed = 09877,
            //        Thst = new HstData
            //        {
            //            Hpid = _clientContext.UserId,
            //            Hslt = 0
            //        },
            //        Uuid = Guid.NewGuid().ToString(),
            //        Voip = 1,
            //        VersionString = "Skate3-1",
            //        Xnnc = null,
            //        Xses = null
            //    },
            //    Mmid = 0,
            //    Pros = new List<PlayerData>
            //    {
            //        new PlayerData
            //        {
            //            Blob = null,
            //            ExternalId = _clientContext.ExternalId,
            //            Gid = gameId,
            //            Locale = 1701729619, //enUS
            //            Username = _clientContext.Username,
            //            NetworkData = new QosNetworkData(),
            //            PlayerAttributes = new Dictionary<string, string>
            //            {
            //                {"dlc_mask", "1"}
            //            },
            //            Pid = _clientContext.UserId, //TODO should be ProfileId
            //            PlayerNetwork = new KeyValuePair<NetworkAddressType, PairNetworkAddress>(
            //                NetworkAddressType.Pair, new PairNetworkAddress
            //                {
            //                    ExternalIp = new ClientNetworkAddress
            //                    {
            //                        Ip = 2130706433,
            //                        Port = 10000
            //                    },
            //                    InternalIp = new ClientNetworkAddress
            //                    {
            //                        Ip = 2130706433,
            //                        Port = 10000
            //                    },
            //                }),
            //            Sid = 1,
            //            Slot = 0,
            //            Stat = 4,
            //            Team = 65535,
            //            Tidx = 65535,
            //            Time = 10000000,
            //            Uid = _clientContext.UserId
            //        }
            //    }
            //});


            //await _notificationHandler.EnqueueNotification(_clientContext.UserId, new GameStateChangeNotification
            //{
            //    GameId = gameId,
            //    State = GameState.PreGame
            //});

            return response;
        }
    }
}