using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.Util.Messages;

[BlazeRequest(BlazeComponent.Util, (ushort)UtilCommand.ClientMetrics)]
[UsedImplicitly]
public record ClientMetricsRequest : BlazeRequestMessage, IRequest<ClientMetricsResponse>
{
    [TdfField("UDEV")]
    public string UpnpDeviceInfo { get; init; }

    [TdfField("USTA")]
    public int UpnpStatus { get; init; }
}