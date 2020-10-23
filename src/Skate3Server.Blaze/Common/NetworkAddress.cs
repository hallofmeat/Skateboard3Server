using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Common
{
    public class ServerNetworkAddress
    {
        [TdfField("HOST")]
        public string Host { get; set; }

        [TdfField("IP")]
        public uint Ip { get; set; } //ip converted to a int

        [TdfField("PORT")]
        public ushort Port { get; set; }
    }

    public class ClientNetworkAddress
    {

        [TdfField("IP")]
        public uint Ip { get; set; } //ip converted to a int

        [TdfField("PORT")]
        public ushort Port { get; set; }
    }

    public class PairNetworkAddress
    {

        [TdfField("EXIP")]
        public ClientNetworkAddress ExternalIp { get; set; }

        [TdfField("INIP")]
        public ClientNetworkAddress InternalIp { get; set; }
    }

}