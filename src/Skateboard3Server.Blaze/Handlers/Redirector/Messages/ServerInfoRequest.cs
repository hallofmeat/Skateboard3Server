using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.Redirector.Messages;

[BlazeRequest(BlazeComponent.Redirector, (ushort)RedirectorCommand.ServerInfo)]
[UsedImplicitly]
public record ServerInfoRequest : BlazeRequestMessage, IRequest<ServerInfoResponse>
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

    [TdfField("FPID")]
    public KeyValuePair<FirstPartyIdType, byte[]> FirstPartyId { get; init; }

    [TdfField("LOC")]
    public uint Locale { get; init; }

    [TdfField("NAME")]
    public string Name { get; init; }
        
    [TdfField("PLAT")]
    public string Platform { get; init; }

    [TdfField("PROF")]
    public string Profile { get; init; }

}