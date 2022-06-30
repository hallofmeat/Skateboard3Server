using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#pragma warning disable CS8618

namespace Skateboard3Server.Data.Models;

public class Persona
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint Id { get; set; } //PersonaId

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; }

    [Required]
    public uint UserId { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public ulong ExternalId { get; set; }

    [Required]
    public PersonaExternalIdType ExternalIdType { get; set; }

    [Required]
    public byte[] ExternalBlob { get; set; }

    [Required]
    public uint LastUsed { get; set; }

}

public enum PersonaExternalIdType
{
    Unknown,
    Xbox,
    Ps3,
    Xenia,
    Rpcs3
}