using GameVault.Data;
using GameVault.Data.Models;
using GameVault.Data.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GameVault.Tests;

public class RepositoryTests : IDisposable
{
    private readonly SqliteConnection connection;
    private readonly GameVaultDbContext context;
    private readonly IIgraRepository igre;
    private readonly IZanrRepository zanrovi;
    private readonly IPlatformaRepository platforme;

    public RepositoryTests()
    {
        connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=True");
        connection.Open();
        context = new GameVaultDbContext(new DbContextOptionsBuilder<GameVaultDbContext>()
            .UseSqlite(connection).Options);
        context.Database.Migrate();
        igre = new IgraRepository(context);
        zanrovi = new ZanrRepository(context);
        platforme = new PlatformaRepository(context);
    }

    [Fact]
    public async Task Igra_DodavanjeIDohvatanje()
    {
        var igra = new Igra { Naziv = "Portal", Opis = "Logicka igra", GodinaIzdanja = 2007 };
        await igre.DodajAsync(igra);
        context.ChangeTracker.Clear();

        var sacuvana = await igre.DohvatiPoIdAsync(igra.Id);
        Assert.NotNull(sacuvana);
        Assert.True(igra.Id > 0);
        Assert.Equal(igra.Naziv, sacuvana.Naziv);
        Assert.Equal(igra.Opis, sacuvana.Opis);
        Assert.Equal(igra.GodinaIzdanja, sacuvana.GodinaIzdanja);
        Assert.True(await igre.PostojiAsync(igra.Id));
        Assert.Single(await igre.DohvatiSveAsync());
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task Igra_IzmenaCuvaSvaPolja()
    {
        var igra = new Igra { Naziv = "Portal" };
        await igre.DodajAsync(igra);
        var izmena = (await igre.DohvatiPoIdAsync(igra.Id))!;
        izmena.Naziv = "Portal 2";
        izmena.Opis = "Novi opis";
        izmena.GodinaIzdanja = 2011;
        izmena.Developer = "Valve";
        izmena.Izdavac = "Valve";
        izmena.Status = StatusIgre.Zavrsena;
        izmena.Ocena = 10;
        izmena.BrojSati = 15;
        izmena.Omiljena = true;
        izmena.DatumDodavanja = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        Assert.True(await igre.IzmeniAsync(izmena));
        context.ChangeTracker.Clear();
        var sacuvana = (await igre.DohvatiPoIdAsync(igra.Id))!;
        Assert.Equal(izmena.Naziv, sacuvana.Naziv);
        Assert.Equal(izmena.Opis, sacuvana.Opis);
        Assert.Equal(izmena.GodinaIzdanja, sacuvana.GodinaIzdanja);
        Assert.Equal(izmena.Developer, sacuvana.Developer);
        Assert.Equal(izmena.Izdavac, sacuvana.Izdavac);
        Assert.Equal(izmena.Status, sacuvana.Status);
        Assert.Equal(izmena.Ocena, sacuvana.Ocena);
        Assert.Equal(izmena.BrojSati, sacuvana.BrojSati);
        Assert.Equal(izmena.Omiljena, sacuvana.Omiljena);
        Assert.Equal(izmena.DatumDodavanja, sacuvana.DatumDodavanja);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Igra_UcitavaVezePoIzboru(bool ukljuciVeze)
    {
        var igra = await DodajIgruSaVezama();
        context.ChangeTracker.Clear();

        var sacuvana = (await igre.DohvatiPoIdAsync(igra.Id, ukljuciVeze))!;
        var izListe = Assert.Single(await igre.DohvatiSveAsync(ukljuciVeze));
        foreach (var rezultat in new[] { sacuvana, izListe })
        {
            Assert.Equal(ukljuciVeze ? 1 : 0, rezultat.Zanrovi.Count);
            Assert.Equal(ukljuciVeze ? 1 : 0, rezultat.Platforme.Count);
            if (ukljuciVeze)
            {
                Assert.Equal("Logicka", Assert.Single(rezultat.Zanrovi).Naziv);
                Assert.Equal("PC", Assert.Single(rezultat.Platforme).Naziv);
            }
        }
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task Igra_IzmenaZamenjujeIUklanjaVezeBezIzmeneSifarnika()
    {
        var igra = await DodajIgruSaVezama();
        var noviZanr = new Zanr { Naziv = "Avantura" };
        var novaPlatforma = new Platforma { Naziv = "PlayStation" };
        await zanrovi.DodajAsync(noviZanr);
        await platforme.DodajAsync(novaPlatforma);
        context.ChangeTracker.Clear();

        var izmena = (await igre.DohvatiPoIdAsync(igra.Id))!;
        izmena.Zanrovi = new List<Zanr> { new() { Id = noviZanr.Id, Naziv = "Ne menjati" } };
        izmena.Platforme = new List<Platforma> { new() { Id = novaPlatforma.Id, Naziv = "Ne menjati" } };
        Assert.True(await igre.IzmeniAsync(izmena));
        context.ChangeTracker.Clear();

        var sacuvana = (await igre.DohvatiPoIdAsync(igra.Id))!;
        Assert.Equal("Avantura", Assert.Single(sacuvana.Zanrovi).Naziv);
        Assert.Equal("PlayStation", Assert.Single(sacuvana.Platforme).Naziv);
        Assert.Equal(2, (await zanrovi.DohvatiSveAsync()).Count);
        Assert.Equal(2, (await platforme.DohvatiSveAsync()).Count);

        sacuvana.Zanrovi.Clear();
        sacuvana.Platforme.Clear();
        Assert.True(await igre.IzmeniAsync(sacuvana));
        context.ChangeTracker.Clear();
        sacuvana = (await igre.DohvatiPoIdAsync(igra.Id))!;
        Assert.Empty(sacuvana.Zanrovi);
        Assert.Empty(sacuvana.Platforme);
    }

    [Fact]
    public async Task Igra_BrisanjeUklanjaVezeIZadrzavaSifarnike()
    {
        var igra = await DodajIgruSaVezama();
        context.ChangeTracker.Clear();
        Assert.True(await igre.ObrisiAsync(igra.Id));
        context.ChangeTracker.Clear();

        Assert.Null(await igre.DohvatiPoIdAsync(igra.Id));
        Assert.False(await igre.PostojiAsync(igra.Id));
        Assert.Empty(await igre.DohvatiSveAsync());
        Assert.Single(await zanrovi.DohvatiSveAsync());
        Assert.Single(await platforme.DohvatiSveAsync());
        Assert.Equal(0, await context.Database.SqlQueryRaw<int>(
            "SELECT COUNT(*) AS Value FROM IgraZanr").SingleAsync());
        Assert.Equal(0, await context.Database.SqlQueryRaw<int>(
            "SELECT COUNT(*) AS Value FROM IgraPlatforma").SingleAsync());
    }

    [Fact]
    public async Task NepostojeciZapisi_VracajuNullIliFalse()
    {
        Assert.Null(await igre.DohvatiPoIdAsync(123));
        Assert.False(await igre.PostojiAsync(123));
        Assert.False(await igre.IzmeniAsync(new Igra { Id = 123, Naziv = "Nepostojeca" }));
        Assert.False(await igre.ObrisiAsync(123));
        Assert.Null(await zanrovi.DohvatiPoIdAsync(123));
        Assert.False(await zanrovi.PostojiPoNazivuAsync("Nepostojeci"));
        Assert.Null(await platforme.DohvatiPoIdAsync(123));
        Assert.False(await platforme.PostojiPoNazivuAsync("Nepostojeca"));
    }

    [Fact]
    public async Task Zanr_DodavanjePronalazenjeILista()
    {
        var zanr = new Zanr { Naziv = "RPG" };
        await zanrovi.DodajAsync(zanr);
        await zanrovi.DodajAsync(new Zanr { Naziv = "Avantura" });
        context.ChangeTracker.Clear();

        Assert.Equal("RPG", (await zanrovi.DohvatiPoIdAsync(zanr.Id))!.Naziv);
        Assert.True(await zanrovi.PostojiPoNazivuAsync("RPG"));
        Assert.False(await zanrovi.PostojiPoNazivuAsync("rpg"));
        Assert.Equal(new[] { "Avantura", "RPG" }, (await zanrovi.DohvatiSveAsync()).Select(z => z.Naziv));
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Fact]
    public async Task Platforma_DodavanjePronalazenjeILista()
    {
        var platforma = new Platforma { Naziv = "PC" };
        await platforme.DodajAsync(platforma);
        await platforme.DodajAsync(new Platforma { Naziv = "Android" });
        context.ChangeTracker.Clear();

        Assert.Equal("PC", (await platforme.DohvatiPoIdAsync(platforma.Id))!.Naziv);
        Assert.True(await platforme.PostojiPoNazivuAsync("PC"));
        Assert.False(await platforme.PostojiPoNazivuAsync("pc"));
        Assert.Equal(new[] { "Android", "PC" }, (await platforme.DohvatiSveAsync()).Select(p => p.Naziv));
        Assert.Empty(context.ChangeTracker.Entries());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Igra_NepostojeceVezeNeCuvajuDelimicnePromene(bool izmena)
    {
        var igra = new Igra { Naziv = "Portal" };
        if (izmena)
            await igre.DodajAsync(igra);
        context.ChangeTracker.Clear();
        igra.Naziv = "Izmenjen naziv";
        igra.Zanrovi.Add(new Zanr { Id = 123 });

        await Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            if (izmena)
                await igre.IzmeniAsync(igra);
            else
                await igre.DodajAsync(igra);
        });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        if (izmena)
            Assert.Equal("Portal", (await igre.DohvatiPoIdAsync(igra.Id))!.Naziv);
        else
            Assert.Empty(await igre.DohvatiSveAsync());
    }

    private async Task<Igra> DodajIgruSaVezama()
    {
        var zanr = new Zanr { Naziv = "Logicka" };
        var platforma = new Platforma { Naziv = "PC" };
        await zanrovi.DodajAsync(zanr);
        await platforme.DodajAsync(platforma);
        var igra = new Igra
        {
            Naziv = "Portal",
            Zanrovi = new List<Zanr> { new() { Id = zanr.Id } },
            Platforme = new List<Platforma> { new() { Id = platforma.Id } }
        };
        await igre.DodajAsync(igra);
        return igra;
    }

    public void Dispose()
    {
        context.Dispose();
        connection.Dispose();
    }
}
