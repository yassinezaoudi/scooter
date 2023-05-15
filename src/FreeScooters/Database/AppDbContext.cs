using FreeScooters.Models;
using Microsoft.EntityFrameworkCore;

namespace FreeScooters.Database;

public class AppDbContext : DbContext
{
    public DbSet<Scooter> Scooters { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}