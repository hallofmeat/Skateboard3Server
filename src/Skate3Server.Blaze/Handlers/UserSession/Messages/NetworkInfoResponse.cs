using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.UserSession.Messages
{
    [BlazeResponse(BlazeComponent.UserSession, 0x14)]
    public class NetworkInfoResponse : BlazeResponse
    {
    }
}
