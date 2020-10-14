using System;
using Skate3Server.Blaze.Serializer.Attributes;

namespace Skate3Server.Blaze.Handlers.Util.Messages
{
    public class PingResponse
    {
        [TdfField("STIM")]
        public uint Timestamp { get; set; }
    }
}
