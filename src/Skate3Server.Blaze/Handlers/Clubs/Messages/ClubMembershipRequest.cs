using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Clubs.Messages
{
    [BlazeRequest(BlazeComponent.Clubs, (ushort)ClubsCommand.ClubMembership)]
    public class ClubMembershipRequest : IRequest<ClubMembershipResponse>
    {
        [TdfField("IDLT")]
        public List<uint> Idlt { get; set; } //TODO

    }
}
