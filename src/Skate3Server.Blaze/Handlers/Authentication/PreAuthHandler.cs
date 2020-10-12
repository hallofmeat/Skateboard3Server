using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Skate3Server.Blaze.Handlers.Authentication.Messages;

namespace Skate3Server.Blaze.Handlers.Authentication
{
    public class PreAuthHandler : IRequestHandler<PreAuthRequest, PreAuthResponse>
    {
        public async Task<PreAuthResponse> Handle(PreAuthRequest request, CancellationToken cancellationToken)
        {
            var response = new PreAuthResponse
            {
                //0x01, 0x04, 0x07, 0x09, 0x0B, 0x0C, 0x0F, 0x19, 0x7800, 0x7802, 0x7803
                ComponentIds = new List<ushort>
                {
                    (ushort) BlazeComponent.Authentication,
                    (ushort) BlazeComponent.GameManager,
                    (ushort) BlazeComponent.Stats,
                    (ushort) BlazeComponent.Util,
                    (ushort) BlazeComponent.Unknown0B,
                    (ushort) BlazeComponent.Unknown0C,
                    (ushort) BlazeComponent.Unknown0F,
                    (ushort) BlazeComponent.Unknown19,
                    (ushort) BlazeComponent.Unknown7800,
                    (ushort) BlazeComponent.Unknown7802,
                    (ushort) BlazeComponent.Unknown7803
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
                        Hostname = "localhost",
                        Port = 17502,
                        Ip = "127.0.0.1"
                    },
                    PingNodeCount = 1, //default is 10
                    PingServers = new Dictionary<string, QosAddress>
                    {
                        //default has 3 servers (lets see if it works with just one)
                        { "test", new QosAddress
                        {
                            Hostname = "localhost",
                            Port = 17502,
                            Ip = "127.0.0.1"
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