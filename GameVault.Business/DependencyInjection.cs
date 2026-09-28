using GameVault.Data;
using GameVault.Business.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameVault.Business;

public static class DependencyInjection
{
    public static IServiceCollection AddBusiness(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("GameVault")
            ?? throw new InvalidOperationException("Nedostaje konekcioni string GameVault.");

        services.AddData(connectionString);
        services.AddScoped<IIgraService, IgraService>();
        services.AddScoped<IStatistikaService, StatistikaService>();
        services.AddScoped<IZanrService, ZanrService>();
        services.AddScoped<IPlatformaService, PlatformaService>();

        return services;
    }
}
