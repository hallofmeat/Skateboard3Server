namespace Skateboard3Server.Blaze
{
    public class BlazeConfig
    {
        public string PublicHost { get; set; }
        public string PublicIp { get; set; }

        public BlazeQosConfig Qos { get; set; }
    }

    public class BlazeQosConfig
    {
        public string BandwidthHost { get; set; }
        public string BandwidthIp { get; set; }
        public ushort BandwidthPort { get; set; }

        public string PingHost { get; set; }
        public string PingIp { get; set; }
        public ushort PingPort { get; set; }
    }
}
