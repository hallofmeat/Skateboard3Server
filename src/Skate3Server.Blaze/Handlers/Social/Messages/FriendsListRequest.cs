using System.Collections.Generic;
using MediatR;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Social.Messages
{
    [BlazeRequest(BlazeComponent.Social, (ushort)SocialCommand.FriendsList)]
    public class FriendsListRequest : IRequest<FriendsListResponse>
    {
        [TdfField("ALST")]
        public List<RequestList> RequestLists { get; set; } //TODO
    }

    public class RequestList
    {
        [TdfField("ALNM")]
        public string Name { get; set; } //TODO

        [TdfField("SUBF")]
        public bool SubF { get; set; } //TODO
    }
}
