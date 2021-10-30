using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skateboard3Server.Data.Models
{
    public class Persona
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint Id { get; set; } //PersonaId

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public uint UserId { get; set; }
        
        public string Username { get; set; }

        [Required]
        public ulong ExternalId { get; set; }

        public PersonaExternalIdType ExternalIdType { get; set; }

        public byte[] ExternalBlob { get; set; }

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

}