using Microsoft.EntityFrameworkCore;
using Skate3Server.Data.Models;

namespace Skate3Server.Data
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
