using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Common
{
    public class QosNetworkData
    {
        [TdfField("DBPS")]
        public uint DownstreamBitsPerSecond { get; set; }

        [TdfField("NATT")]
        public NatType NatType { get; set; }

        [TdfField("UBPS")]
        public uint UpstreamBitsPerSecond { get; set; }

    }
}