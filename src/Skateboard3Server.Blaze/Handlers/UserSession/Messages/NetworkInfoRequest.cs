using System.Collections.Generic;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.UserSession.Messages
{
    [BlazeRequest(BlazeComponent.UserSession, (ushort)UserSessionCommand.NetworkInfo)]
    public class NetworkInfoRequest : BlazeRequest, IRequest<NetworkInfoResponse>
    {
        [TdfField("ADDR")]
        public KeyValuePair<NetworkAddressType, PairNetworkAddress> Address { get; set; }

        [TdfField("NLMP")]
        public Dictionary<string, int> Pings { get; set; }

        [TdfField("NQOS")]
        public QosNetworkData NetworkData { get; set; }

    }
}
