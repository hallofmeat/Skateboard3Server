using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.Authentication.Messages;

[BlazeRequest(BlazeComponent.Authentication, (ushort)AuthenticationCommand.Login)]
[UsedImplicitly]
public record LoginRequest : BlazeRequestMessage, IRequest<LoginResponse>
{
    [TdfField("MAIL")]
    public string Email { get; init; }

    [TdfField("TCKT")]
    public byte[] Ticket { get; init; }
}