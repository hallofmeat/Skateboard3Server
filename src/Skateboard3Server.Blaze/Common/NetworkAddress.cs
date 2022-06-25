using Skateboard3Server.Blaze.Serializer.Attributes;

namespace Skateboard3Server.Blaze.Common;

public record NetworkAddress
{
}

public record ServerNetworkAddress : NetworkAddress
{
    [TdfField("HOST")]
    public string Host { get; set; }

    [TdfField("IP")]
    public uint Ip { get; set; } //ip converted to a int

    [TdfField("PORT")]
    public ushort Port { get; set; }
}

public record ClientNetworkAddress : NetworkAddress
{

    [TdfField("IP")]
    public uint Ip { get; set; } //ip converted to a int

    [TdfField("PORT")]
    public ushort Port { get; set; }
}

public record PairNetworkAddress : NetworkAddress
{

    [TdfField("EXIP")]
    public ClientNetworkAddress ExternalIp { get; set; }

    [TdfField("INIP")]
    public ClientNetworkAddress InternalIp { get; set; }
}