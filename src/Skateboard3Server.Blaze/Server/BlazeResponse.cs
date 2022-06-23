namespace Skateboard3Server.Blaze.Server;

public abstract class BlazeResponse : IBlazeMessage
{
    public ushort BlazeErrorCode { get; set; } //TODO: not sure if I want to put this here
}