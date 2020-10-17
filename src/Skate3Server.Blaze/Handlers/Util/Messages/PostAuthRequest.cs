using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Util.Messages
{
    [BlazeRequest(BlazeComponent.Util, 0x8)]
    public class PostAuthRequest : IRequest<PostAuthResponse>
    {
        //Empty
    }
}
