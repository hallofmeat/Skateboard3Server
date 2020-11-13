using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Util.Messages
{
    [BlazeRequest(BlazeComponent.Util, (ushort)UtilCommand.PostAuth)]
    public class PostAuthRequest : IRequest<PostAuthResponse>, IBlazeRequest
    {
        //Empty
    }
}
