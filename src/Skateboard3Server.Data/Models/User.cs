using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skateboard3Server.Data.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public uint Id { get; set; } //BlazeId

        public long AccountId { get; set; }
        
        //public uint AccountLocale { get; set; }

        public long PersonaId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public ulong ExternalId { get; set; }

        public UserExternalIdType ExternalIdType { get; set; }

        public byte[] ExternalBlob { get; set; }

        public uint LastLogin { get; set; }

    }

    public enum UserExternalIdType
    {
        Unknown,
        Xbox,
        PS3,
        Xenia,
        Rpcs3
    }

}