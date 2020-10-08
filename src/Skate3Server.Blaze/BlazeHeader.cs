namespace Skate3Server.Blaze
{
    public class BlazeHeader
    {
        public int Length { get; set; }
        
        public BlazeComponent Component { get; set; }
        
        public int Command { get; set; }
        
        public int ErrorCode { get; set; }

        public BlazeMessageType MessageType { get; set; }

        public int MessageId { get; set; }
    }
}
