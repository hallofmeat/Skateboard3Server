using System.Collections.Generic;
using Skate3Server.Blaze.Serializer.Attributes;
using Skate3Server.Blaze.Server;

namespace Skate3Server.Blaze.Handlers.Social.Messages
{
    [BlazeResponse(BlazeComponent.Social, 0x6)]
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
