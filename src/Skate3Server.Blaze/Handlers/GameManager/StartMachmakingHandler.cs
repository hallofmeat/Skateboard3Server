using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Handlers.GameManager.Messages;
using Skate3Server.Blaze.Notifications.GameManager;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.GameManager
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
            uint gameId = 12345; //TODO
            uint msid = 123; //TODO
            var response = new StartMatchmakingResponse
            {
                Msid = msid //TODO
            };

            await _notificationHandler.EnqueueNotification(_clientContext.UserId, new MatchmakingStatusNotification
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
                Msid = msid,
                Usid = 123456
            });

            await _notificationHandler.EnqueueNotification(_clientContext.UserId, new MatchmakingFinishedNotification
            {
                Fit = 100,
                GameId = gameId,
                Maxf = 100,
                Msid = msid,
                Result = MatchmakingResult.TimedOut,
                Usid = 123456
            });
            //TODO: we are just creating a game with just the user in it
            //_clientContext.Notifications.Enqueue((new BlazeHeader
            //{
            //    Component = BlazeComponent.GameManager,
            //    Command = (ushort)GameManagerNotification.GameSetup,
            //    MessageId = 0,
            //    MessageType = BlazeMessageType.Notification,
            //    ErrorCode = 0
            //}, new GameSetupNotification
            //{
            //    Error = 0,
            //    GameData = new GameData
            //    {
            //        Admins = new List<uint> { _clientContext.UserId },
            //        Attributes = request.Attributes,
            //        Cap = new List<ushort> { 6 , 0 },
            //        GameId = gameId,
            //        Gnam = request.Gnam,
            //        Gpvh = 1000000000,
            //        Gset = request.Gset,
            //        Gsid = 98765,
            //        GameState = GameState.Init,
            //        Gver = 1,
            //        Hnet = new List<KeyValuePair<NetworkAddressType, ClientNetworkAddress>>{
            //            new KeyValuePair<NetworkAddressType, ClientNetworkAddress>(NetworkAddressType.IpAddress, new ClientNetworkAddress
            //            {
            //                Ip = 2130706433,
            //                Port = 10000
            //            })},
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
            //        Psas = "sjc",
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
            //            PlayerNetwork = new KeyValuePair<NetworkAddressType, PairNetworkAddress>(NetworkAddressType.Pair, new PairNetworkAddress
            //            {
            //                ExternalIp = new ClientNetworkAddress
            //                {
            //                    Ip = 2130706433,
            //                    Port = 10000
            //                },
            //                InternalIp = new ClientNetworkAddress
            //                {
            //                    Ip = 2130706433,
            //                    Port = 10000
            //                },
            //            }),
            //            Sid = 1,
            //            Slot = 0,
            //            Stat = 4,
            //            Team = 65535,
            //            Tidx = 65535,
            //            Time = 10000000,
            //            Uid = 123456
            //        }
            //    }
            //}));
            //_clientContext.Notifications.Enqueue((new BlazeHeader
            //{
            //    Component = BlazeComponent.GameManager,
            //    Command = (ushort)GameManagerNotification.GameStateChange,
            //    MessageId = 0,
            //    MessageType = BlazeMessageType.Notification,
            //    ErrorCode = 0
            //}, new GameStateChangeNotification
            //{
            //    GameId = gameId,
            //    State = GameState.PreGame
            //}));

            return response;
        }
    }
}