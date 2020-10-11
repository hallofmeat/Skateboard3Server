namespace Skate3Server.Blaze
{
    public class BlazeHeader
    {
        public ushort Length { get; set; }
        
        public BlazeComponent Component { get; set; }
        
        public ushort Command { get; set; }
        
        public ushort ErrorCode { get; set; }

        public BlazeMessageType MessageType { get; set; }

        public int MessageId { get; set; }
    }
}
