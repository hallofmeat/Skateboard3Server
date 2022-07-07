using System.Collections.Generic;
using JetBrains.Annotations;
using MediatR;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

#pragma warning disable CS8618

namespace Skateboard3Server.Blaze.Handlers.Teams.Messages;

[BlazeRequest(BlazeComponent.Teams, (ushort)TeamsCommand.TeamMembers)]
[UsedImplicitly]
public record TeamMembersRequest : BlazeRequestMessage, IRequest<TeamMembersResponse>
{
    [TdfField("CLID")]
    public uint Clid { get; init; } //TODO

}