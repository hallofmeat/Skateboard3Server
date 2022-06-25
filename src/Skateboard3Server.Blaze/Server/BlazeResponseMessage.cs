namespace Skateboard3Server.Blaze.Server;

public abstract record BlazeResponseMessage : IBlazeMessage
{
    public ushort BlazeErrorCode { get; set; }
}