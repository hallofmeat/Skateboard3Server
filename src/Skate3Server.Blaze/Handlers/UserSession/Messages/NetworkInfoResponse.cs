using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.UserSession.Messages
{
    [BlazeResponse(BlazeComponent.UserSession, 0x14)]
    public class NetworkInfoResponse : BlazeResponse
    {
    }
}
