using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeRequest(BlazeComponent.Authentication, (ushort)AuthenticationCommand.Login)]
    public class LoginRequest : BlazeRequest, IRequest<LoginResponse>
    {
        [TdfField("MAIL")]
        public string Email { get; set; }

        [TdfField("TCKT")]
        public byte[] Ticket { get; set; }
    }
}
