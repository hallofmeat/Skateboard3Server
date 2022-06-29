using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.Authentication.Messages;

[BlazeRequest(BlazeComponent.Authentication, (ushort)AuthenticationCommand.Dlc)]
[UsedImplicitly]
public record DlcRequest : BlazeRequestMessage, IRequest<DlcResponse>
{
    [TdfField("BUID")]
    public uint BlazeId { get; init; }

    [TdfField("EPSN")]
    public ushort Epsn { get; init; } //TODO

    [TdfField("EPSZ")]
    public ushort Epsz { get; init; } //TODO

    [TdfField("GNLS")]
    public List<string> Gnls { get; init; } //TODO

    [TdfField("OLD")]
    public bool Old { get; init; } //TODO

    [TdfField("ONLY")]
    public bool Only { get; init; } //TODO

    [TdfField("PERS")]
    public bool Pers { get; init; } //TODO
}