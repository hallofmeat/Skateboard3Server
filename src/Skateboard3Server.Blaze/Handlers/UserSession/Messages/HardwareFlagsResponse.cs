using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.UserSession.Messages
{
    [BlazeResponse(BlazeComponent.UserSession, (ushort)UserSessionCommand.HardwareFlags)]
    public class HardwareFlagsResponse : IBlazeResponse
    {
        //Empty
    }
}
