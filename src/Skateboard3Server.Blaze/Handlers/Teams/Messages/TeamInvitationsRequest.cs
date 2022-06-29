using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Teams.Messages;

[BlazeRequest(BlazeComponent.Teams, (ushort)TeamsCommand.TeamInvitations)]
[UsedImplicitly]
public record TeamInvitationsRequest : BlazeRequestMessage, IRequest<TeamInvitationsResponse>
{
    [TdfField("CLID")]
    public uint ClientId { get; init; }

    [TdfField("INVT")]
    public int Invt { get; init; } //TODO

    [TdfField("NSOT")]
    public int Nsot { get; init; } //TODO

}