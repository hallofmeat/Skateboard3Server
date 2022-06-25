using System.Collections.Generic;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.UserSession.Messages;

[BlazeResponse(BlazeComponent.UserSession, (ushort)UserSessionCommand.LookupUsers)]
public record LookupUsersResponse : BlazeResponseMessage
{

    [TdfField("ULST")]
    public List<UserInformation> Users { get; set; }
}