using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Util.Messages;

[BlazeRequest(BlazeComponent.Util, (ushort)UtilCommand.Ping)]
[UsedImplicitly]
public record PingRequest : BlazeRequestMessage, IRequest<PingResponse>
{
}