using System.Buffers;

namespace Skate3Server.Blaze.Server
{
    public class BlazeMessage
    {
        public BlazeHeader Header { get; set; }
        
        public ReadOnlySequence<byte> Payload { get; set; }
    }
}
