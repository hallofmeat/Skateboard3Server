using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Util.Messages
{
    [BlazeRequest(BlazeComponent.Util, (ushort)UtilCommand.ClientMetrics)]
    public class ClientMetricsRequest : IRequest<ClientMetricsResponse>
    {
        [TdfField("UDEV")]
        public string UpnpDeviceInfo { get; set; }

        [TdfField("USTA")]
        public int UpnpStatus { get; set; }
    }
}
