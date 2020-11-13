using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeResponse(BlazeComponent.Authentication, (ushort)AuthenticationCommand.Dlc)]
    public class DlcResponse : IBlazeResponse
    {
    }
}
