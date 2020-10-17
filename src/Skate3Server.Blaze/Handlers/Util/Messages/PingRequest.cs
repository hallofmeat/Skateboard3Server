using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Util.Messages
{
    [BlazeRequest(BlazeComponent.Util, 0x2)]
    public class PingRequest : IRequest<PingResponse>
    {
    }
}
