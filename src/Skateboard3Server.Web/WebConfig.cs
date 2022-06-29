using System.ComponentModel.DataAnnotations;

#pragma warning disable CS8618

namespace Skateboard3Server.Web;

public class WebConfig
{
    [Required]
    public string BlobStorageLocation { get; set; }
}