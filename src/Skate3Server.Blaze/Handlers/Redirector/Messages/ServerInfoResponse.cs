using System.Collections.Generic;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Redirector.Messages
{
    [BlazeResponse(BlazeComponent.Redirector, 0x1)]
    public class ServerInfoResponse : BlazeResponse
    {
        //Need to be in order
        [TdfField("ADDR")]
        public KeyValuePair<NetworkAddressType, NetworkAddress> Address { get; set; }

        [TdfField("SECU")]
        public bool Secure { get; set; }

        [TdfField("XDNS")]
        public uint Xdns { get; set; }

    }

    public class NetworkAddress
    {
        [TdfField("HOST")]
        public string Host { get; set; }

        [TdfField("IP")]
        public uint Ip { get; set; } //ip converted to a int

        [TdfField("PORT")]
        public ushort Port { get; set; }
    }
}
