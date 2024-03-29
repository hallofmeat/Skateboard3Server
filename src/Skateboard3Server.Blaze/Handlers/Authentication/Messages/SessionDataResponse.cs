﻿using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.Authentication.Messages;

[BlazeResponse(BlazeComponent.Authentication, (ushort)AuthenticationCommand.SessionData)]
public record SessionDataResponse : BlazeResponseMessage
{
    [TdfField("BUID")]
    public uint BlazeId { get; set; }

    [TdfField("FRST")]
    public bool FirstLogin { get; set; }

    [TdfField("KEY")]
    public string SessionKey { get; set; }

    [TdfField("LLOG")]
    public long LastLoginTime { get; set; }

    [TdfField("MAIL")]
    public string Email { get; set; }

    [TdfField("PDTL")]
    public SessionDataPersona Persona { get; set; }

    [TdfField("UID")]
    public long AccountId { get; set; }
}

public record SessionDataPersona
{
    [TdfField("DSNM")]
    public string DisplayName { get; set; }

    [TdfField("LAST")]
    public uint LastUsed { get; set; }

    [TdfField("PID")]
    public long PersonaId { get; set; }

    [TdfField("XREF")]
    public ulong ExternalId { get; set; }

    [TdfField("XTYP")]
    public ExternalIdType ExternalIdType { get; set; }
}