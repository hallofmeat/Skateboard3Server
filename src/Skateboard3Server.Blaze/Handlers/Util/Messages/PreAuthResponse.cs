using System.Collections.Generic;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Util.Messages;

[BlazeResponse(BlazeComponent.Util, (ushort)UtilCommand.PreAuth)]
public class PreAuthResponse : BlazeResponse
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
    public ushort PingCount { get; set; }

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