using GameVault.Data.Repositories;
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

        services.AddScoped<IIgraRepository, IgraRepository>();
        services.AddScoped<IZanrRepository, ZanrRepository>();
        services.AddScoped<IPlatformaRepository, PlatformaRepository>();

        return services;
    }
}
