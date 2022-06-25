using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Util.Messages;

[BlazeRequest(BlazeComponent.Util, (ushort)UtilCommand.PostAuth)]
[UsedImplicitly]
public record PostAuthRequest : BlazeRequestMessage, IRequest<PostAuthResponse>
{
    //Empty
}