using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Util.Messages;

namespace Skate3Server.Blaze.Handlers.Util
{
    public class PreAuthHandler : IRequestHandler<PreAuthRequest, PreAuthResponse>
    {
        public async Task<PreAuthResponse> Handle(PreAuthRequest request, CancellationToken cancellationToken)
        {
            var response = new PreAuthResponse
            {
                //0x01, 0x04, 0x07, 0x08, 0x09, 0x0B, 0x0C, 0x0F, 0x19, 0x7800, 0x7802, 0x7803
                //We only care about some of these but send all of them just incase we need to handle them
                ComponentIds = new List<ushort>
                {
                    (ushort) BlazeComponent.Authentication,
                    (ushort) BlazeComponent.GameManager,
                    (ushort) BlazeComponent.Stats,
                    0x08,
                    (ushort) BlazeComponent.Util,
                    (ushort) BlazeComponent.Teams,
                    (ushort) BlazeComponent.SkateStats,
                    0x0F,
                    (ushort) BlazeComponent.Social,
                    0x7800,
                    (ushort) BlazeComponent.UserSession,
                    0x7803
                },
                ServerConfig = new ServerConfig
                {
                    Values = new Dictionary<string, string>
                    {
                        { "pingPeriodInMs", "15000" },
                        { "voipHeadsetUpdateRate", "1000" }
                    }
                },
                QosConfig = new QosConfig
                {
                    BandwidthServer = new QosAddress
                    {
                        Hostname = BlazeConfig.BlazeHost,
                        Port = 17502,
                        Ip = BlazeConfig.BlazeIp
                    },
                    PingNodeCount = 1, //default is 10
                    PingServers = new Dictionary<string, QosAddress>
                    {
                        //default has 3 servers (lets see if it works with just one)
                        { "tst", new QosAddress
                        {
                            Hostname = BlazeConfig.BlazeHost,
                            Port = 17502,
                            Ip = BlazeConfig.BlazeIp
                        }}
                    },
                    ServerId = 1

                },
                ServerVersion = "Skate3Server 0.0.1"
            };
            return response;
        }
    }
}