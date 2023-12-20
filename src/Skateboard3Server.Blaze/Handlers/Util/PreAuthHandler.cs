using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Skateboard3Server.Blaze.Handlers.Util.Messages;
using Skateboard3Server.Common.Config;

namespace Skateboard3Server.Blaze.Handlers.Util;

public class PreAuthHandler : IRequestHandler<PreAuthRequest, PreAuthResponse>
{
    private readonly BlazeConfig _blazeConfig;

    public PreAuthHandler(IOptions<BlazeConfig> blazeConfig)
    {
        _blazeConfig = blazeConfig.Value;
    }
    public Task<PreAuthResponse> Handle(PreAuthRequest request, CancellationToken cancellationToken)
    {
        var pingQosHosts = new Dictionary<string, QosAddress>();
        var bandwidthServer = new QosAddress
        {
            Hostname = "",
            Ip = "",
            Port = 17502 //default 17502
        };
        if (_blazeConfig.QosHosts != null)
        {
            pingQosHosts = _blazeConfig.QosHosts.Select(x => new
            {
                Key = x.Name,
                Value = new QosAddress
                {
                    Hostname = x.Host,
                    Ip = x.Ip,
                    Port = x.Port,
                }
            }).ToDictionary(pair => pair.Key, pair => pair.Value);

            var firstQosHost = _blazeConfig.QosHosts.FirstOrDefault();

            if (firstQosHost != null)
            {
                bandwidthServer.Hostname = firstQosHost.Host;
                bandwidthServer.Ip = firstQosHost.Ip;
                bandwidthServer.Port = firstQosHost.Port;
            }
        }

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
                BandwidthServer = bandwidthServer,
                PingCount = 10,
                PingServers = pingQosHosts,
                ServerId = 1
            },
            ServerVersion = "Skateboard3Server 0.0.1"
        };
        return Task.FromResult(response);
    }
}