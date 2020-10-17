using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeRequest(BlazeComponent.Authentication, 0xC8)]
    public class LoginRequest : IRequest<LoginResponse>
    {
        [TdfField("MAIL")]
        public string Email { get; set; }

        [TdfField("TCKT")]
        public byte[] Ticket { get; set; }
    }
}
