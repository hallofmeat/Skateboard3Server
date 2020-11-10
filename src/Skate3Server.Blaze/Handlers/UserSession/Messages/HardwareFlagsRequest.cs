using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.UserSession.Messages
{
    [BlazeRequest(BlazeComponent.UserSession, (ushort)UserSessionCommand.HardwareFlags)]
    public class HardwareFlagsRequest : IRequest<HardwareFlagsResponse>
    {
        [TdfField("HWFG")]
        public uint HardwareFlags { get; set; }
    }
}
