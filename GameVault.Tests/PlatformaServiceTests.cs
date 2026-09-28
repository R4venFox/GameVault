using GameVault.Business;
using GameVault.Business.Services;
using GameVault.Data.Models;
using GameVault.Tests.Fakes;
using Xunit;

namespace GameVault.Tests;

public class PlatformaServiceTests
{
    private readonly PlatformaRepositoryFake repository = new();
    private readonly PlatformaService service;

    public PlatformaServiceTests()
    {
        service = new PlatformaService(repository);
    }

    [Fact]
    public async Task DodavanjeUklanjaOkolneRazmake()
    {
        var zapis = await service.DodajAsync("  Primer  ");
        Assert.Equal("Primer", zapis.Naziv);
        Assert.Same(zapis, Assert.Single(repository.Zapisi));
        Assert.Same(zapis, Assert.Single(await service.DohvatiSveAsync()));
        Assert.False(await service.PostojiPoNazivuAsync("Drugi"));
    }

    [Theory]
    [InlineData("Primer")]
    [InlineData("PRIMER")]
    [InlineData("primer")]
    [InlineData("  pRiMeR  ")]
    public async Task DuplikatiSeOdbijajuBezObziraNaVelicinuSlova(string naziv)
    {
        await service.DodajAsync("Primer");
        Assert.True(await service.PostojiPoNazivuAsync(naziv));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.DodajAsync(naziv));
        Assert.Single(repository.Zapisi);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t\n ")]
    public async Task PrazanNazivSeOdbija(string? naziv)
    {
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.DodajAsync(naziv!));
        Assert.False(await service.PostojiPoNazivuAsync(naziv!));
        Assert.Empty(repository.Zapisi);
    }

    [Fact]
    public async Task PostojeciZapisiSaRazmacimaSePrepoznaju()
    {
        repository.Zapisi.Add(new Platforma { Id = 1, Naziv = "  Žanr  " });
        Assert.True(await service.PostojiPoNazivuAsync("žANR"));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.DodajAsync("ŽANR"));
    }
}
