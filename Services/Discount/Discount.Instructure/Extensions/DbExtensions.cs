using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discount.Instructure.Extensions;

public static class DbExtensions
{
    public static IHost MigrateDatabase<TContext>(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;
        var config = services.GetRequiredService<IConfiguration>();
        var logger = services.GetRequiredService<ILogger<TContext>>();
        try
        {
            logger.LogInformation("discount db migration stared");
            ApplyMigration(config);
            logger.LogInformation("discount db migration completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
        return host;
    }

    private static void ApplyMigration(IConfiguration config)
    {
        throw new NotImplementedException();
    }
}
