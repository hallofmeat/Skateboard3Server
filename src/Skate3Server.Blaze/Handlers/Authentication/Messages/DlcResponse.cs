using Skate3Server.Blaze.Common;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Authentication.Messages
{
    [BlazeResponse(BlazeComponent.Authentication, 0x20)]
    public class DlcResponse : BlazeResponse
    {
    }
}
