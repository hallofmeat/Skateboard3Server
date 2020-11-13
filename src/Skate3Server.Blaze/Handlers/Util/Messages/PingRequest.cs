using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Util.Messages
{
    [BlazeRequest(BlazeComponent.Util, (ushort)UtilCommand.Ping)]
    public class PingRequest : IRequest<PingResponse>, IBlazeRequest
    {
    }
}
