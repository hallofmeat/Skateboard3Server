using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Common;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.UserSession.Messages;

[BlazeRequest(BlazeComponent.UserSession, (ushort)UserSessionCommand.LookupUsers)]
[UsedImplicitly]
public record LookupUsersRequest : BlazeRequestMessage, IRequest<LookupUsersResponse>
{
    [TdfField("LTYP")]
    public UserLookupType LookupType { get; init; }

    [TdfField("ULST")]
    public List<UserInformation> Users { get; init; }
}