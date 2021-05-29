using System.Collections.Generic;
using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Social.Messages
{
    [BlazeResponse(BlazeComponent.Social, (ushort)SocialCommand.FriendsList)]
    public class FriendsListResponse : BlazeResponse
    {
        [TdfField("ALMP")]
        public Dictionary<string, ResponseList> ResponseLists { get; set; }
    }

    public class ResponseList
    {
        [TdfField("BOID")]
        public ulong Boid { get; set; } //TODO

        [TdfField("LID")]
        public uint Lid { get; set; } //TODO

        [TdfField("LMS")]
        public uint Lms { get; set; } //TODO

        [TdfField("LMN")]
        public string Name { get; set; }

        [TdfField("RFLG")]
        public bool RFlag { get; set; } //TODO

        [TdfField("SFLG")]
        public bool SFlag { get; set; } //TODO
    }
}
