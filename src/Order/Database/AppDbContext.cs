using Microsoft.EntityFrameworkCore;
using Order.Models;

namespace Order.Database;

public class AppDbContext : DbContext
{
    public DbSet<Rent> Rents { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}