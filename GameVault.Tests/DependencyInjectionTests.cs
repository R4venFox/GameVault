using GameVault.Business;
using GameVault.Business.Services;
using GameVault.Data;
using GameVault.Data.Models;
using GameVault.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace GameVault.Tests;

public class DependencyInjectionTests
{
    [Fact]
    public void AddBusiness_RegistrujeScopedServise()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:GameVault"] = "Data Source=:memory:"
            }).Build();
        var services = new ServiceCollection();
        services.AddBusiness(configuration);
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });
        using var scope = provider.CreateScope();
        using var drugiScope = provider.CreateScope();
        var igre = scope.ServiceProvider.GetRequiredService<IIgraService>();
        var statistika = scope.ServiceProvider.GetRequiredService<IStatistikaService>();
        Assert.IsType<StatistikaService>(statistika);
        Assert.Same(statistika, scope.ServiceProvider.GetRequiredService<IStatistikaService>());
        Assert.NotSame(statistika, drugiScope.ServiceProvider.GetRequiredService<IStatistikaService>());
        var zanrovi = scope.ServiceProvider.GetRequiredService<IZanrService>();
        var platforme = scope.ServiceProvider.GetRequiredService<IPlatformaService>();
        Assert.IsType<IgraService>(igre);
        Assert.IsType<ZanrService>(zanrovi);
        Assert.IsType<PlatformaService>(platforme);
        Assert.Same(igre, scope.ServiceProvider.GetRequiredService<IIgraService>());
        Assert.Same(zanrovi, scope.ServiceProvider.GetRequiredService<IZanrService>());
        Assert.Same(platforme, scope.ServiceProvider.GetRequiredService<IPlatformaService>());
        Assert.NotSame(igre, drugiScope.ServiceProvider.GetRequiredService<IIgraService>());
        Assert.NotSame(zanrovi, drugiScope.ServiceProvider.GetRequiredService<IZanrService>());
        Assert.NotSame(platforme, drugiScope.ServiceProvider.GetRequiredService<IPlatformaService>());
    }

    [Fact]
    public async Task AddBusiness_RegistrujeRepositoryKlaseSaZajednickimScopedContextom()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:GameVault"] = "Data Source=:memory:"
            }).Build();
        var services = new ServiceCollection();
        services.AddBusiness(configuration);
        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });
        using var scope = provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<GameVaultDbContext>();
        await context.Database.OpenConnectionAsync();
        await context.Database.MigrateAsync();
        var igre = scope.ServiceProvider.GetRequiredService<IIgraRepository>();
        var zanrovi = scope.ServiceProvider.GetRequiredService<IZanrRepository>();
        var platforme = scope.ServiceProvider.GetRequiredService<IPlatformaRepository>();

        Assert.IsType<IgraRepository>(igre);
        Assert.IsType<ZanrRepository>(zanrovi);
        Assert.IsType<PlatformaRepository>(platforme);
        Assert.Same(igre, scope.ServiceProvider.GetRequiredService<IIgraRepository>());
        Assert.Same(zanrovi, scope.ServiceProvider.GetRequiredService<IZanrRepository>());
        Assert.Same(platforme, scope.ServiceProvider.GetRequiredService<IPlatformaRepository>());
        var zanr = new Zanr { Naziv = "Logicka" };
        var platforma = new Platforma { Naziv = "PC" };
        await zanrovi.DodajAsync(zanr);
        await platforme.DodajAsync(platforma);
        await igre.DodajAsync(new Igra
        {
            Naziv = "Portal",
            Zanrovi = new List<Zanr> { zanr },
            Platforme = new List<Platforma> { platforma }
        });
        Assert.Single(await context.Igre.ToListAsync());

        using var drugiScope = provider.CreateScope();
        Assert.NotSame(igre, drugiScope.ServiceProvider.GetRequiredService<IIgraRepository>());
        Assert.NotSame(zanrovi, drugiScope.ServiceProvider.GetRequiredService<IZanrRepository>());
        Assert.NotSame(platforme, drugiScope.ServiceProvider.GetRequiredService<IPlatformaRepository>());
    }

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
