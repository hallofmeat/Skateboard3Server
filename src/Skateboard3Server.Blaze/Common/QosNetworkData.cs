using Skateboard3Server.Blaze.Serializer.Attributes;

namespace Skateboard3Server.Blaze.Common;

public record QosNetworkData
{
    [TdfField("DBPS")]
    public uint DownstreamBitsPerSecond { get; init; }

    [TdfField("NATT")]
    public NatType NatType { get; init; }

    [TdfField("UBPS")]
    public uint UpstreamBitsPerSecond { get; init; }

}