using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Util.Messages
{
    [BlazeResponse(BlazeComponent.Util, 0x2)]
    public class PingResponse : BlazeResponse
    {
        [TdfField("STIM")]
        public uint Timestamp { get; set; }
    }
}
