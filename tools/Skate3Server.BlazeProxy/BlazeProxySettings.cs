namespace Skate3Server.BlazeProxy
{
    public class BlazeProxySettings
    {
        public int LocalPort { get; set; }
        public int RemotePort { get; set; }
        public string RemoteHost { get; set; }
        public bool Secure { get; set; }
    }
}
