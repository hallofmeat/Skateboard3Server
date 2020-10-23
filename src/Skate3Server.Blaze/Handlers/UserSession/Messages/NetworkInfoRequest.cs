using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.UserSession.Messages
{
    [BlazeRequest(BlazeComponent.UserSession, 0x14)]
    public class NetworkInfoRequest : IRequest<NetworkInfoResponse>
    {
        [TdfField("ADDR")]
        public KeyValuePair<NetworkAddressType, PairNetworkAddress> Address { get; set; }

        [TdfField("NLMP")]
        public Dictionary<string, int> Pings { get; set; }

        [TdfField("NQOS")]
        public QosNetworkData NetworkData { get; set; }

    }
}
