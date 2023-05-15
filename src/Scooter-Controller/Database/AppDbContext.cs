using Microsoft.EntityFrameworkCore;
using Scooter_Controller.Models;

namespace Scooter_Controller.Database;

public class AppDbContext : DbContext
{
    public DbSet<Scooter> Scooters { get; set; }
    public DbSet<SpaceTime> SpaceTimes { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}