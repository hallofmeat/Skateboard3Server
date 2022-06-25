using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Teams.Messages;

[BlazeRequest(BlazeComponent.Teams, (ushort)TeamsCommand.Unknown640)]
[UsedImplicitly]
public record Unknown640Request : BlazeRequestMessage, IRequest<Unknown640Response>
{
    [TdfField("CLID")]
    public uint ClientId { get; set; }

    [TdfField("INVT")]
    public int Invt { get; set; } //TODO

    [TdfField("NSOT")]
    public int Nsot { get; set; } //TODO

}