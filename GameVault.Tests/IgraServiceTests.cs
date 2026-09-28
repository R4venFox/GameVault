using GameVault.Business;
using GameVault.Business.Models;
using GameVault.Business.Services;
using GameVault.Data.Models;
using GameVault.Tests.Fakes;
using Xunit;

namespace GameVault.Tests;

public class IgraServiceTests
{
    private readonly IgraRepositoryFake igre = new();
    private readonly ZanrRepositoryFake zanrovi = new();
    private readonly PlatformaRepositoryFake platforme = new();
    private readonly IgraService service;

    public IgraServiceTests()
    {
        zanrovi.Zapisi.Add(new Zanr { Id = 1, Naziv = "Logicka" });
        platforme.Zapisi.Add(new Platforma { Id = 1, Naziv = "PC" });
        service = new IgraService(igre, zanrovi, platforme);
    }

    private static IgraPodaci ValidniPodaci() => new()
    {
        Naziv = " Portal ", Beleske = "Probati ponovo", GodinaIzdanja = 2007, Ocena = 10, BrojSati = 5,
        ZanrIds = new List<int> { 1 }, PlatformaIds = new List<int> { 1 }
    };

    [Fact]
    public async Task DodavanjeValidneIgre_CuvaPodatkeIVeze()
    {
        var preDodavanja = DateTime.UtcNow;
        var igra = await service.DodajAsync(ValidniPodaci());
        Assert.Same(igra, Assert.Single(igre.Zapisi));
        Assert.Equal("Portal", igra.Naziv);
        Assert.Equal("Probati ponovo", igra.Beleske);
        Assert.Equal(2007, igra.GodinaIzdanja);
        Assert.Equal(10, igra.Ocena);
        Assert.Equal(5, igra.BrojSati);
        Assert.Equal("Logicka", Assert.Single(igra.Zanrovi).Naziv);
        Assert.Equal("PC", Assert.Single(igra.Platforme).Naziv);
        Assert.InRange(igra.DatumDodavanja, preDodavanja, DateTime.UtcNow);
    }

    [Theory]
    [InlineData("naziv-null")]
    [InlineData("naziv-prazan")]
    [InlineData("naziv-razmaci")]
    [InlineData("sati")]
    [InlineData("ocena-0")]
    [InlineData("ocena-11")]
    [InlineData("godina-stara")]
    [InlineData("godina-buduca")]
    [InlineData("status")]
    [InlineData("zanr-ne-postoji")]
    [InlineData("platforma-ne-postoji")]
    [InlineData("zanr-duplikat")]
    [InlineData("platforma-duplikat")]
    [InlineData("zanrovi-null")]
    [InlineData("platforme-null")]
    public async Task NevalidniPodaci_OdbijajuDodavanjeIIzmenu(string slucaj)
    {
        var postojeca = await service.DodajAsync(ValidniPodaci());
        var podaci = ValidniPodaci();
        switch (slucaj)
        {
            case "naziv-null": podaci.Naziv = null!; break;
            case "naziv-prazan": podaci.Naziv = ""; break;
            case "naziv-razmaci": podaci.Naziv = " \t\n "; break;
            case "sati": podaci.BrojSati = -1; break;
            case "ocena-0": podaci.Ocena = 0; break;
            case "ocena-11": podaci.Ocena = 11; break;
            case "godina-stara": podaci.GodinaIzdanja = 1949; break;
            case "godina-buduca": podaci.GodinaIzdanja = DateTime.UtcNow.Year + 6; break;
            case "status": podaci.Status = (StatusIgre)99; break;
            case "zanr-ne-postoji": podaci.ZanrIds = new() { 99 }; break;
            case "platforma-ne-postoji": podaci.PlatformaIds = new() { 99 }; break;
            case "zanr-duplikat": podaci.ZanrIds = new() { 1, 1 }; break;
            case "platforma-duplikat": podaci.PlatformaIds = new() { 1, 1 }; break;
            case "zanrovi-null": podaci.ZanrIds = null!; break;
            case "platforme-null": podaci.PlatformaIds = null!; break;
        }
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.DodajAsync(podaci));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.IzmeniAsync(postojeca.Id, podaci));
        Assert.Single(igre.Zapisi);
        Assert.Equal(0, igre.BrojIzmena);
        Assert.Equal("Portal", postojeca.Naziv);
        Assert.Equal(1, Assert.Single(postojeca.Zanrovi).Id);
        Assert.Equal(1, Assert.Single(postojeca.Platforme).Id);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData(1, 1950)]
    [InlineData(10, 2007)]
    public async Task GranicneIOpcioneVrednostiSuDozvoljene(int? ocena, int? godina)
    {
        await service.DodajAsync(new IgraPodaci { Naziv = "Igra", Ocena = ocena, GodinaIzdanja = godina });
        await service.DodajAsync(new IgraPodaci { Naziv = "Najavljena", GodinaIzdanja = DateTime.UtcNow.Year + 5 });
        Assert.Equal(2, igre.Zapisi.Count);
    }

    [Fact]
    public async Task IzmenaCuvaDatumDodavanjaIZamenjujeVeze()
    {
        var igra = await service.DodajAsync(ValidniPodaci());
        var datum = igra.DatumDodavanja;
        var podaci = new IgraPodaci
        {
            Naziv = " Nova igra ", Opis = "Opis", Beleske = "Zavrsiti izazove", Developer = "Studio", Izdavac = "Izdavac",
            Status = StatusIgre.UToku, GodinaIzdanja = 2020, Ocena = 8, BrojSati = 20, Omiljena = true
        };
        await service.IzmeniAsync(igra.Id, podaci);
        Assert.Equal("Nova igra", igra.Naziv);
        Assert.Equal("Opis", igra.Opis);
        Assert.Equal("Zavrsiti izazove", igra.Beleske);
        Assert.Equal("Studio", igra.Developer);
        Assert.Equal("Izdavac", igra.Izdavac);
        Assert.Equal(datum, igra.DatumDodavanja);
        Assert.Equal(StatusIgre.UToku, igra.Status);
        Assert.Equal(2020, igra.GodinaIzdanja);
        Assert.Equal(8, igra.Ocena);
        Assert.Equal(20, igra.BrojSati);
        Assert.True(igra.Omiljena);
        Assert.Empty(igra.Zanrovi);
        Assert.Empty(igra.Platforme);
        Assert.Equal(1, igre.BrojIzmena);
    }

    [Theory]
    [InlineData(StatusIgre.Planirana)]
    [InlineData(StatusIgre.UToku)]
    [InlineData(StatusIgre.Zavrsena)]
    [InlineData(StatusIgre.Napustena)]
    public async Task PromenaStatusaCuvaVeze(StatusIgre status)
    {
        var igra = await service.DodajAsync(ValidniPodaci());
        await service.PromeniStatusAsync(igra.Id, status);
        Assert.Equal(status, igra.Status);
        Assert.Single(igra.Zanrovi);
        Assert.Single(igra.Platforme);
        Assert.True(igre.UkljuceneVeze);
        Assert.Equal(1, igre.BrojIzmena);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(1)]
    [InlineData(10)]
    public async Task PromenaOcene(int? ocena)
    {
        var igra = await service.DodajAsync(ValidniPodaci());
        await service.PromeniOcenuAsync(igra.Id, ocena);
        Assert.Equal(ocena, igra.Ocena);
        Assert.Equal(1, igre.BrojIzmena);
        Assert.Single(igra.Zanrovi);
        Assert.Single(igra.Platforme);
    }

    [Fact]
    public async Task PromenaSatiIOmiljenih()
    {
        var igra = await service.DodajAsync(ValidniPodaci());
        await service.PromeniBrojSatiAsync(igra.Id, 0);
        Assert.Equal(0, igra.BrojSati);
        await service.PromeniBrojSatiAsync(igra.Id, 12);
        Assert.Equal(12, igra.BrojSati);
        await service.PostaviOmiljenuAsync(igra.Id, true);
        Assert.True(igra.Omiljena);
        await service.PostaviOmiljenuAsync(igra.Id, false);
        Assert.False(igra.Omiljena);
        Assert.Single(igra.Zanrovi);
        Assert.Single(igra.Platforme);
        Assert.Equal(4, igre.BrojIzmena);
    }

    [Fact]
    public async Task NevalidnePojedinacnePromeneNeCuvajuPodatke()
    {
        var igra = await service.DodajAsync(ValidniPodaci());
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.PromeniOcenuAsync(igra.Id, 0));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.PromeniOcenuAsync(igra.Id, 11));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.PromeniBrojSatiAsync(igra.Id, -1));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.PromeniStatusAsync(igra.Id, (StatusIgre)99));
        Assert.Equal(0, igre.BrojIzmena);
    }

    [Fact]
    public async Task NepostojecaIgraImaDosledneGreske()
    {
        Assert.Null(await service.DohvatiPoIdAsync(99));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.IzmeniAsync(99, ValidniPodaci()));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.ObrisiAsync(99));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.PromeniStatusAsync(99, StatusIgre.UToku));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.PromeniOcenuAsync(99, 5));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.PromeniBrojSatiAsync(99, 5));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.PostaviOmiljenuAsync(99, true));
    }

    [Fact]
    public async Task DohvatanjeIBrisanje()
    {
        var igra = await service.DodajAsync(ValidniPodaci());
        Assert.Same(igra, await service.DohvatiPoIdAsync(igra.Id));
        Assert.Same(igra, Assert.Single(await service.DohvatiSveAsync()));
        Assert.True(igre.UkljuceneVeze);
        await service.ObrisiAsync(igra.Id);
        Assert.Empty(igre.Zapisi);
    }

    [Fact]
    public async Task NestanakIgreTokomIzmenePrijavljujeGresku()
    {
        var igra = await service.DodajAsync(ValidniPodaci());
        igre.OdbijIzmenu = true;
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => service.IzmeniAsync(igra.Id, ValidniPodaci()));
    }
}
