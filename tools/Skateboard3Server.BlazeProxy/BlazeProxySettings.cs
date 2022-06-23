namespace Skateboard3Server.BlazeProxy;

public class BlazeProxySettings
{ 
    public string RedirectHost { get; set; }
    public string RedirectIp { get; set; }
    public int LocalPort { get; set; }
    public int RemotePort { get; set; }
    public string RemoteHost { get; set; }
    public bool Secure { get; set; }
}