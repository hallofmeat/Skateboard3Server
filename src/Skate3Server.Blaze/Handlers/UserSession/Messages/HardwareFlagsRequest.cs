using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.UserSession.Messages
{
    [BlazeRequest(BlazeComponent.UserSession, 0x08)]
    public class HardwareFlagsRequest : IRequest<HardwareFlagsResponse>
    {
        [TdfField("HWFG")]
        public uint HardwareFlags { get; set; }
    }
}
