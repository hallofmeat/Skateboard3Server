﻿using Skateboard3Server.Blaze.Serializer.Attributes;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Common;

public record UserInformation
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