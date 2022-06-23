using System.Collections.Generic;

namespace Skateboard3Server.Blaze;

public class BlazeConfig
{
    public string PublicHost { get; set; }
    public string PublicIp { get; set; }

    public List<QosHostsConfig> QosHosts { get; set; }
}

public class QosHostsConfig
{
    public string Name { get; set; }
    public string Host { get; set; }
    public string Ip { get; set; }
    public ushort Port { get; set; }
}