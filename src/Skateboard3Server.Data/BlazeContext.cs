using Microsoft.EntityFrameworkCore;
using Skateboard3Server.Data.Models;

namespace Skateboard3Server.Data
{
    public class BlazeContext : DbContext
    {
        public BlazeContext(DbContextOptions<BlazeContext> options)
            : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
    }
}
