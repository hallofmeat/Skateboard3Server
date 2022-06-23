using System.Collections.Generic;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.UserSession.Messages;

[BlazeRequest(BlazeComponent.UserSession, (ushort)UserSessionCommand.LookupUsers)]
public class LookupUsersRequest : BlazeRequest, IRequest<LookupUsersResponse>
{
    [TdfField("LTYP")]
    public int ListType { get; set; } //TODO: enum

    [TdfField("ULST")]
    public List<UserList> Users { get; set; }
}

public class UserList
{
    [TdfField("AID")]
    public long AccountId { get; set; }

    [TdfField("ALOC")]
    public uint AccountLocale { get; set; }

    [TdfField("EXBB")]
    public byte[] ExternalBlob { get; set; }

    [TdfField("EXID")]
    public ulong ExternalId { get; set; }

    [TdfField("ID")]
    public uint Id { get; set; }

    [TdfField("NAME")]
    public string Username { get; set; }

    [TdfField("ONLN")]
    public bool Online { get; set; }

    [TdfField("PID")]
    public long PersonaId { get; set; }
}