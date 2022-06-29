using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.Teams.Messages;

[BlazeRequest(BlazeComponent.Teams, (ushort)TeamsCommand.TeamMembership)]
[UsedImplicitly]
public record TeamMembershipRequest : BlazeRequestMessage, IRequest<TeamMembershipResponse>
{
    [TdfField("IDLT")]
    public List<uint> Idlt { get; init; } //TODO

}