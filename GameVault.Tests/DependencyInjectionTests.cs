using GameVault.Business;
using GameVault.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GameVault.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public async Task AddBusiness_PovezujeSqliteKrozDependencyInjection()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:GameVault"] = "Data Source=:memory:"
            })
            .Build();
        var services = new ServiceCollection();
        services.AddBusiness(configuration);

        using var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GameVaultDbContext>();

        await context.Database.OpenConnectionAsync();
        using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = "SELECT 1";

        Assert.Equal(1L, await command.ExecuteScalarAsync());
        Assert.Same(context, scope.ServiceProvider.GetRequiredService<GameVaultDbContext>());

        using var secondScope = provider.CreateScope();
        Assert.NotSame(context, secondScope.ServiceProvider.GetRequiredService<GameVaultDbContext>());
    }

    [Fact]
    public void AddBusiness_BezKonekcionogStringa_PrijavljujeGresku()
    {
        var configuration = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        Assert.Throws<InvalidOperationException>(() => services.AddBusiness(configuration));
    }
}
