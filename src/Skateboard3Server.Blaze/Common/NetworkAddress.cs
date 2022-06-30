using Skateboard3Server.Blaze.Serializer.Attributes;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Common;

public record NetworkAddress;

public record ServerNetworkAddress : NetworkAddress
{
    [TdfField("HOST")]
    public string Host { get; init; }

    [TdfField("IP")]
    public uint Ip { get; init; } //ip converted to a int

    [TdfField("PORT")]
    public ushort Port { get; init; }
}

public record ClientNetworkAddress : NetworkAddress
{

    [TdfField("IP")]
    public uint Ip { get; init; } //ip converted to a int

    [TdfField("PORT")]
    public ushort Port { get; init; }
}

public record PairNetworkAddress : NetworkAddress
{

    [TdfField("EXIP")]
    public ClientNetworkAddress ExternalIp { get; init; }

    [TdfField("INIP")]
    public ClientNetworkAddress InternalIp { get; init; }
}