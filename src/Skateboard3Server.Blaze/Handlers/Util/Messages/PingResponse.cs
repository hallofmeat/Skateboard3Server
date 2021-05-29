using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Util.Messages
{
    [BlazeResponse(BlazeComponent.Util, (ushort)UtilCommand.Ping)]
    public class PingResponse : BlazeResponse
    {
        [TdfField("STIM")]
        public uint Timestamp { get; set; }
    }
}
