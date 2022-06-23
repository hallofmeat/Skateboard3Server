using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Authentication.Messages;

[BlazeRequest(BlazeComponent.Authentication, (ushort)AuthenticationCommand.SessionData)]
public class SessionDataRequest : BlazeRequest, IRequest<SessionDataResponse>
{
}