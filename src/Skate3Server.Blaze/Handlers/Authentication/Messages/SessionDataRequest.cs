using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeRequest(BlazeComponent.Authentication, (ushort)AuthenticationCommand.SessionData)]
    public class SessionDataRequest : IRequest<SessionDataResponse>
    {
    }
}
