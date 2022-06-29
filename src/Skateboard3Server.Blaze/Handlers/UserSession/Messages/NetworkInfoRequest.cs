using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.UserSession.Messages;

[BlazeRequest(BlazeComponent.UserSession, (ushort)UserSessionCommand.NetworkInfo)]
[UsedImplicitly]
public record NetworkInfoRequest : BlazeRequestMessage, IRequest<NetworkInfoResponse>
{
    [TdfField("ADDR")]
    public KeyValuePair<NetworkAddressType, PairNetworkAddress> Address { get; init; }

    [TdfField("NLMP")]
    public Dictionary<string, int> Pings { get; init; }

    [TdfField("NQOS")]
    public QosNetworkData NetworkData { get; init; }

}