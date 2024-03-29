﻿using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.Util.Messages;

[BlazeResponse(BlazeComponent.Util, (ushort)UtilCommand.PostAuth)]
public record PostAuthResponse : BlazeResponseMessage
{
    [TdfField("TELE")]
    public TelemetryServer TelemetryServer { get; set; }
        
    [TdfField("TICK")]
    public TickServer TickServer { get; set; }

}

public record TelemetryServer
{
    [TdfField("ADRS")]
    public string Ip { get; set; }

    [TdfField("ANON")]
    public bool Anonymous { get; set; }

    [TdfField("DISA")]
    public string Disable { get; set; }

    [TdfField("FILT")]
    public string Filter { get; set; }

    [TdfField("LOC")]
    public uint Locale { get; set; }

    [TdfField("NOOK")]
    public string Nook { get; set; } //TODO

    [TdfField("PORT")]
    public uint Port { get; set; }

    [TdfField("SDLY")]
    public uint Delay { get; set; }

    [TdfField("SKEY")]
    public string Key { get; set; }

    [TdfField("SPCT")]
    public uint Spct { get; set; } //TODO
}

public record TickServer
{
    [TdfField("ADRS")]
    public string Ip { get; set; }

    [TdfField("PORT")]
    public uint Port { get; set; }

    [TdfField("SKEY")]
    public string Key { get; set; }

}