using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Authentication.Messages;

[BlazeResponse(BlazeComponent.Authentication, (ushort)AuthenticationCommand.Dlc)]
public record DlcResponse : BlazeResponseMessage
{
    //Empty
}