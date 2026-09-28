using GameVault.Data;
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

        return services;
    }
}
