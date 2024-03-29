using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Inventory.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Inventory.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = YourBrand.Inventory.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Inventory")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<InventoryContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IInventoryContext>(sp => sp.GetRequiredService<InventoryContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}