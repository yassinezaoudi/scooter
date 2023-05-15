using Gateway.Domain;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure;

public class AppDbContext : DbContext, IDataProtectionKeyContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}