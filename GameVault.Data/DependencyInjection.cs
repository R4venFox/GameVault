using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GameVault.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(
        this IServiceCollection services, string connectionString)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<GameVaultDbContext>(options =>
            options.UseSqlite(connectionString));

        return services;
    }
}
