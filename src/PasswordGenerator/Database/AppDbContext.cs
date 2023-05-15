using Microsoft.EntityFrameworkCore;
using Password_Generator.Models;

namespace Password_Generator.Database;

public sealed class AppDbContext : DbContext
{
    public DbSet<OneTimePassword> OneTimePasswords { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}