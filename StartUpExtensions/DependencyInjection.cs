using Microsoft.EntityFrameworkCore;

namespace dotnet_core_boilerplate.StartUpExtensions;

public static partial class StartupExtensions
{
    public static void ConfigureDependencyInjection(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DbContext>(option =>
        {
            option.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
        });

        AddBackgroundTasks(services);
    }

    private static void AddBackgroundTasks(IServiceCollection services)
    {
        // Prefer to keep API running when a background task fails
    }
}