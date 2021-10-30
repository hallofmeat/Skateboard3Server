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

        public uint LastLogin { get; set; }

    }
}