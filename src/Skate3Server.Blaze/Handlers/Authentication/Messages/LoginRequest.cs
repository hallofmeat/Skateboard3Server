using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeRequest(BlazeComponent.Authentication, (ushort)AuthenticationCommand.Login)]
    public class LoginRequest : IRequest<LoginResponse>, IBlazeRequest
    {
        [TdfField("MAIL")]
        public string Email { get; set; }

        [TdfField("TCKT")]
        public byte[] Ticket { get; set; }
    }
}
