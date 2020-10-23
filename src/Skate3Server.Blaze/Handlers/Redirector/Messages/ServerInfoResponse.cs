using System.Collections.Generic;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Redirector.Messages
{
    [BlazeResponse(BlazeComponent.Redirector, 0x1)]
    public class ServerInfoResponse : BlazeResponse
    {
        //Need to be in order
        [TdfField("ADDR")]
        public KeyValuePair<NetworkAddressType, ServerNetworkAddress> Address { get; set; }

        [TdfField("SECU")]
        public bool Secure { get; set; }

        [TdfField("XDNS")]
        public uint Xdns { get; set; }

    }
}
