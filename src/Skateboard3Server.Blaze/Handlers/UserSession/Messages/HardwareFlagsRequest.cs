using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.UserSession.Messages;

[BlazeRequest(BlazeComponent.UserSession, (ushort)UserSessionCommand.HardwareFlags)]
[UsedImplicitly]
public record HardwareFlagsRequest : BlazeRequestMessage, IRequest<HardwareFlagsResponse>
{
    [TdfField("HWFG")]
    public uint HardwareFlags { get; set; }
}