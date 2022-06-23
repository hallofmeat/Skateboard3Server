using System.Collections.Generic;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Redirector.Messages;

[BlazeResponse(BlazeComponent.Redirector, (ushort)RedirectorCommand.ServerInfo)]
public class ServerInfoResponse : BlazeResponse
{
    //Need to be in order
    [TdfField("ADDR")]
    public KeyValuePair<NetworkAddressType, ServerNetworkAddress> Address { get; set; }

    [TdfField("SECU")]
    public bool Secure { get; set; }

    [TdfField("XDNS")]
    public uint Xdns { get; set; } //TODO: dnsip?

}