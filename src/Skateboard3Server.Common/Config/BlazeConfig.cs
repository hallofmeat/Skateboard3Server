using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace Skateboard3Server.Common.Config;

public class BlazeConfig
{
    [Required]
    public string PublicHost { get; set; }

    [Required]
    public string PublicIp { get; set; }

    public List<QosHostsConfig>? QosHosts { get; set; }
}

public class QosHostsConfig
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string Host { get; set; }

    [Required]
    public string Ip { get; set; }

    [Required]
    public ushort Port { get; set; }
}