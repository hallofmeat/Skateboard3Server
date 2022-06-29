using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.Util.Messages;

[BlazeRequest(BlazeComponent.Util, (ushort)UtilCommand.PreAuth)]
[UsedImplicitly]
public record PreAuthRequest : BlazeRequestMessage, IRequest<PreAuthResponse>
{
    [TdfField("CDAT")]
    public ClientData ClientData { get; init; }

    [TdfField("CINF")]
    public ClientInfo ClientInfo { get; init; }

    [TdfField("FCCR")]
    public ClientConfigData ClientConfig { get; init; }

}

public record ClientData
{
    [TdfField("LANG")]
    public uint Language { get; init; }

    [TdfField("TYPE")]
    public int Type { get; init; }
}

public record ClientInfo
{
    [TdfField("BSDK")]
    public string BlazeSdk { get; init; }

    [TdfField("BTIM")]
    public string BlazeTime { get; init; }

    [TdfField("CLNT")]
    public string ClientId { get; init; }

    [TdfField("CSKU")]
    public string ClientSku { get; init; }

    [TdfField("CVER")]
    public string ClientVersion { get; init; }

    [TdfField("DSDK")]
    public string SdkVersion { get; init; }

    [TdfField("ENV")]
    public string Environment { get; init; }

    [TdfField("LOC")]
    public uint Locale { get; init; }

    [TdfField("MAC")]
    public string MacAddress { get; init; }

    [TdfField("PLAT")]
    public string Platform { get; init; }

}

public record ClientConfigData
{
    [TdfField("CFID")]
    public string ConfigId { get; init; }
}