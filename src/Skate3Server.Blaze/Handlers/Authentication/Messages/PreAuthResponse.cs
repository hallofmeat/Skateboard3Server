using System.Collections.Generic;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    public class PreAuthResponse
    {
        [TdfField("CIDS")]
        public List<ushort> ComponentIds { get; set; }

        [TdfField("CONF")]
        public ServerConfig ServerConfig { get; set; }

        [TdfField("QOSS")]
        public QosConfig QosConfig { get; set; }

        [TdfField("SVER")]
        public string ServerVersion { get; set; }
    }

    public class ServerConfig
    {
        [TdfField("CONF")]
        public Dictionary<string, string> Values { get; set; }
    }

    public class QosConfig
    {
        [TdfField("BWPS")]
        public QosAddress BandwidthServer { get; set; }

        [TdfField("LNP")]
        public ushort PingNodeCount { get; set; }

        [TdfField("LTPS")]
        public Dictionary<string, QosAddress> PingServers { get; set; }

        [TdfField("SVID")]
        public uint ServerId { get; set; }
    }

    public class QosAddress
    {
        [TdfField("PSA")]
        public string Hostname { get; set; }

        [TdfField("PSP")]
        public ushort Port { get; set; }

        [TdfField("SNA")]
        public string Ip { get; set; }
    }



}
