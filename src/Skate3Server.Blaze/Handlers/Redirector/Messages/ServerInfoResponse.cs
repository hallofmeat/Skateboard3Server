using System;
using System.Collections.Generic;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Redirector.Messages
{
    //TODO Order attribute or param for tdffield?
    public class ServerInfoResponse
    {
        //Need to be in order
        [TdfField("ADDR")]
        public KeyValuePair<NetworkAddressType, NetworkAddress> Address { get; set; }

        [TdfField("SECU")]
        public sbyte Secure { get; set; }

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
