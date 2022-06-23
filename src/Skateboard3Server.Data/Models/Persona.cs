using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skateboard3Server.Data.Models;

public class Persona
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public uint Id { get; set; } //PersonaId

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    [Required]
    public uint UserId { get; set; }

    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public ulong ExternalId { get; set; }

    [Required]
    public PersonaExternalIdType ExternalIdType { get; set; }

    [Required]
    public byte[] ExternalBlob { get; set; } = null!;

    [Required]
    public uint LastUsed { get; set; }

}

public enum PersonaExternalIdType
{
    Unknown,
    Xbox,
    PS3,
    Xenia,
    Rpcs3
}