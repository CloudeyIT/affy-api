namespace Affy.Core.Framework.Database;

public static class DatabaseSetupExtensions
{
    public static IServiceCollection AddDatabase (this IServiceCollection services, IConfiguration configuration)
    {
        var migrationsAssembly = configuration.GetValue<string>("MigrationsAssembly", "Affy.Migrations");

        services.AddPooledDbContextFactory<MainDb>(
            builder => builder
                .UseNpgsql(
                    configuration.GetConnectionString(nameof(MainDb)),
                    npgsql =>
                    {
                        npgsql.MigrationsAssembly(migrationsAssembly);
                        npgsql.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
                    }
                )
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
        );

        return services;
    }
}