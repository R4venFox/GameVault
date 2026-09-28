using GameVault.Business;
using GameVault.Business.Services;
using GameVault.Data.Models;
using GameVault.Tests.Fakes;
using Xunit;

namespace GameVault.Tests;

public class ZanrServiceTests
{
    [Fact]
    public async Task IzmenaNazivaCuvaIdIOdbijaDuplikate()
    {
        var prvi = await service.DodajAsync("Prvi");
        var drugi = await service.DodajAsync("Drugi");
        await service.IzmeniAsync(prvi.Id, " PRVI ");
        Assert.Equal("PRVI", (await service.DohvatiPoIdAsync(prvi.Id))!.Naziv);
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.IzmeniAsync(prvi.Id, " drugi "));
        Assert.Equal("PRVI", prvi.Naziv);
        await service.IzmeniAsync(prvi.Id, "Novi naziv");
        Assert.Equal("Novi naziv", prvi.Naziv);
        Assert.Equal("Drugi", drugi.Naziv);
    }

    [Fact]
    public async Task PovezanZapisNeMozeDaSeObrise()
    {
        var zapis = await service.DodajAsync("Primer");
        repository.KorisceniId.Add(zapis.Id);
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.ObrisiAsync(zapis.Id));
        Assert.Single(repository.Zapisi);
        repository.KorisceniId.Clear();
        await service.ObrisiAsync(zapis.Id);
        Assert.Empty(repository.Zapisi);
    }

    [Fact]
    public async Task IzmenaPraznogNazivaINepostojeciZapisiSeOdbijaju()
    {
        var zapis = await service.DodajAsync("Primer");
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.IzmeniAsync(zapis.Id, "   "));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.IzmeniAsync(99, "Novi"));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.ObrisiAsync(99));
        Assert.Equal("Primer", zapis.Naziv);
    }

    private readonly ZanrRepositoryFake repository = new();
    private readonly ZanrService service;

    public ZanrServiceTests()
    {
        service = new ZanrService(repository);
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
        repository.Zapisi.Add(new Zanr { Id = 1, Naziv = "  Žanr  " });
        Assert.True(await service.PostojiPoNazivuAsync("žANR"));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.DodajAsync("ŽANR"));
    }
}
