using System.Buffers;

namespace Skateboard3Server.Blaze.Server;

public class BlazeMessageData
{
    public BlazeHeader Header { get; set; }
        
    public ReadOnlySequence<byte> Payload { get; set; }
}