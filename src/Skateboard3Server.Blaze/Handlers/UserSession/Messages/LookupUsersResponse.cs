using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.UserSession.Messages;

[BlazeResponse(BlazeComponent.UserSession, (ushort)UserSessionCommand.LookupUsers)]
public class LookupUsersResponse : BlazeResponse
{
    //TODO: fill this out
}