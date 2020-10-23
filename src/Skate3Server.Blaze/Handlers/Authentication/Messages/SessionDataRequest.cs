using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeRequest(BlazeComponent.Authentication, 0xE6)]
    public class SessionDataRequest : IRequest<SessionDataResponse>
    {
    }
}
