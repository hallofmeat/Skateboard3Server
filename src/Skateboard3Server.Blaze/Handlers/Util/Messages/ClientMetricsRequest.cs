using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Util.Messages;

[BlazeRequest(BlazeComponent.Util, (ushort)UtilCommand.ClientMetrics)]
public class ClientMetricsRequest : BlazeRequest, IRequest<ClientMetricsResponse>
{
    [TdfField("UDEV")]
    public string UpnpDeviceInfo { get; set; }

    [TdfField("USTA")]
    public int UpnpStatus { get; set; }
}