using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace Skateboard3Server.Common.Config;

public class WebConfig
{
    [Required]
    public string BlobStorageLocation { get; set; }
}