using Calabonga.AspNetCore.AppDefinitions;
using Gateway.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Definitions.DbContext;

public class DbContextDefinition : AppDefinition
{
    /// <summary>
    /// Configure services for current application
    /// </summary>
    /// <param name="services"></param>
    /// <param name="builder"></param>
    public override void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("DEMO"));
        }
        else if (builder.Environment.IsProduction())
        {
            services.AddDbContext<AppDbContext>(options =>
            {
                var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
                options.UseNpgsql(connectionString);
            });
        }
        // services.AddDbContext<AppDbContext>(config =>
        // {
        //     var environment = builder.Environment;
        //     if (environment.IsDevelopment())
        //     {
        //         config.UseInMemoryDatabase("DEMO-PURPOSES-ONLY");
        //     }

        //     if (environment.IsProduction())
        //     {
        //         var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        //         config.UseNpgsql(connectionString);
        //     }
        // });
    }
}