using Skateboard3Server.Blaze.Serializer.Attributes;
using Skateboard3Server.Blaze.Server;

namespace Skateboard3Server.Blaze.Handlers.Util.Messages;

[BlazeResponse(BlazeComponent.Util, (ushort)UtilCommand.Ping)]
public record PingResponse : BlazeResponseMessage
{
    [TdfField("STIM")]
    public uint Timestamp { get; set; }
}