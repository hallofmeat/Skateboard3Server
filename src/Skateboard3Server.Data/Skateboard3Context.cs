using Microsoft.EntityFrameworkCore;
using Skateboard3Server.Data.Models;

#pragma warning disable CS8618

namespace Skateboard3Server.Data;

public class Skateboard3Context : DbContext
{
    public Skateboard3Context(DbContextOptions<Skateboard3Context> options)
        : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Persona> Personas { get; set; }
}