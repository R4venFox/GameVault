using GameVault.Business;
using GameVault.Business.Models;
using GameVault.Business.Services;
using GameVault.Data;
using GameVault.Data.Models;
using GameVault.Data.Repositories;
using GameVault.Web.Controllers;
using GameVault.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GameVault.Tests;

public class PretragaIStatistikaTests : IDisposable
{
    private readonly SqliteConnection connection;
    private readonly GameVaultDbContext context;
    private readonly IgraService igre;
    private readonly StatistikaService statistika;

    public PretragaIStatistikaTests()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        context = new GameVaultDbContext(new DbContextOptionsBuilder<GameVaultDbContext>()
            .UseSqlite(connection).Options);
        context.Database.Migrate();
        var repository = new IgraRepository(context);
        igre = new IgraService(repository, new ZanrRepository(context), new PlatformaRepository(context));
        statistika = new StatistikaService(repository);
    }

    private async Task PopuniAsync()
    {
        var avantura = new Zanr { Id = 1, Naziv = "Avantura" };
        var logicka = new Zanr { Id = 2, Naziv = "Logicka" };
        var pc = new Platforma { Id = 1, Naziv = "PC" };
        var konzola = new Platforma { Id = 2, Naziv = "Konzola" };
        var datum = new DateTime(2026, 1, 1);
        context.Igre.AddRange(
            new Igra { Id = 1, Naziv = "Beta", Developer = "Studio Žar", Izdavac = "Izdavac A",
                Status = StatusIgre.UToku, Ocena = 8, BrojSati = 20, Omiljena = true,
                GodinaIzdanja = 2020, DatumDodavanja = datum.AddDays(2),
                Zanrovi = new List<Zanr> { avantura, logicka }, Platforme = new List<Platforma> { pc, konzola } },
            new Igra { Id = 2, Naziv = "alfa", Developer = "Drugi studio", Izdavac = "Izdavac B",
                Status = StatusIgre.Zavrsena, Ocena = 10, BrojSati = 30, Omiljena = true,
                GodinaIzdanja = 2010, DatumDodavanja = datum,
                Zanrovi = new List<Zanr> { avantura }, Platforme = new List<Platforma> { pc } },
            new Igra { Id = 3, Naziv = "Čarolija", Status = StatusIgre.Planirana,
                DatumDodavanja = datum.AddDays(3),
                Zanrovi = new List<Zanr> { logicka }, Platforme = new List<Platforma> { konzola } },
            new Igra { Id = 4, Naziv = "Delta", Status = StatusIgre.Napustena,
                Ocena = 4, BrojSati = 5, GodinaIzdanja = 2015, DatumDodavanja = datum.AddDays(1) });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
    }

    [Theory]
    [InlineData(" ET ", 1)]
    [InlineData("ŽAR", 1)]
    [InlineData("davac b", 2)]
    [InlineData("čAR", 3)]
    public async Task PretragaPoDeluNazivaDeveloperaIIzdavaca(string tekst, int id)
    {
        await PopuniAsync();
        Assert.Equal(id, Assert.Single(await igre.PretraziAsync(new IgraPretraga { Tekst = tekst })).Id);
    }

    [Theory]
    [InlineData("status", "1")]
    [InlineData("zanr", "2,1")]
    [InlineData("platforma", "1,3")]
    [InlineData("omiljene", "2,1")]
    [InlineData("kombinovani", "1")]
    public async Task FilteriRadePojedinacnoIZajedno(string slucaj, string ocekivaniId)
    {
        await PopuniAsync();
        var upit = slucaj switch
        {
            "status" => new IgraPretraga { Status = StatusIgre.UToku },
            "zanr" => new IgraPretraga { ZanrId = 1 },
            "platforma" => new IgraPretraga { PlatformaId = 2 },
            "omiljene" => new IgraPretraga { SamoOmiljene = true },
            _ => new IgraPretraga { Tekst = "studio", Status = StatusIgre.UToku,
                ZanrId = 1, PlatformaId = 2, SamoOmiljene = true }
        };
        Assert.Equal(ocekivaniId, string.Join(",", (await igre.PretraziAsync(upit)).Select(i => i.Id)));
    }

    [Theory]
    [InlineData(SortiranjeIgara.Naziv, false, "2,1,4,3")]
    [InlineData(SortiranjeIgara.Naziv, true, "3,4,1,2")]
    [InlineData(SortiranjeIgara.Ocena, false, "4,1,2,3")]
    [InlineData(SortiranjeIgara.Ocena, true, "2,1,4,3")]
    [InlineData(SortiranjeIgara.GodinaIzdanja, false, "2,4,1,3")]
    [InlineData(SortiranjeIgara.GodinaIzdanja, true, "1,4,2,3")]
    [InlineData(SortiranjeIgara.BrojSati, false, "3,4,1,2")]
    [InlineData(SortiranjeIgara.BrojSati, true, "2,1,4,3")]
    [InlineData(SortiranjeIgara.DatumDodavanja, false, "2,4,1,3")]
    [InlineData(SortiranjeIgara.DatumDodavanja, true, "3,1,4,2")]
    public async Task SortiranjeRadiUObaSmera(SortiranjeIgara sortiranje, bool opadajuce, string ocekivaniId)
    {
        await PopuniAsync();
        var rezultat = await igre.PretraziAsync(new IgraPretraga { Sortiranje = sortiranje, Opadajuce = opadajuce });
        Assert.Equal(ocekivaniId, string.Join(",", rezultat.Select(i => i.Id)));
    }

    [Fact]
    public async Task BezRezultataIPrazanTekst()
    {
        await PopuniAsync();
        Assert.Empty(await igre.PretraziAsync(new IgraPretraga { Tekst = "ne postoji" }));
        Assert.Empty(await igre.PretraziAsync(new IgraPretraga { ZanrId = 99 }));
        Assert.Empty(await igre.PretraziAsync(new IgraPretraga { Status = StatusIgre.Planirana, SamoOmiljene = true }));
        Assert.Equal(4, (await igre.PretraziAsync(new IgraPretraga { Tekst = "   " })).Count);
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => igre.PretraziAsync(new IgraPretraga { Sortiranje = (SortiranjeIgara)99 }));
        await Assert.ThrowsAsync<PoslovnaGreskaException>(() => igre.PretraziAsync(new IgraPretraga { Status = (StatusIgre)99 }));
    }

    [Fact]
    public async Task StatistikaPrazneBazeIStranica()
    {
        var controller = new StatistikaController(statistika);
        var result = Assert.IsType<ViewResult>(await controller.Index(default));
        var model = Assert.IsType<StatistikaViewModel>(result.Model);
        Assert.Equal(0, model.UkupnoIgara);
        Assert.Equal(0, model.Planirane);
        Assert.Equal(0, model.UToku);
        Assert.Equal(0, model.Zavrsene);
        Assert.Equal(0, model.Napustene);
        Assert.Equal(0, model.Omiljene);
        Assert.Equal(0, model.UkupnoSati);
        Assert.Null(model.ProsecnaOcena);
        Assert.Null(model.NajcesciZanr);
        Assert.Null(model.NajcescaPlatforma);
    }

    [Fact]
    public async Task StatistikaCeleKolekcijeNeZavisiOdFiltera()
    {
        await PopuniAsync();
        await igre.PretraziAsync(new IgraPretraga { Status = StatusIgre.UToku });
        var controller = new StatistikaController(statistika);
        var result = Assert.IsType<ViewResult>(await controller.Index(default));
        var model = Assert.IsType<StatistikaViewModel>(result.Model);
        Assert.Equal(4, model.UkupnoIgara);
        Assert.Equal(1, model.Planirane);
        Assert.Equal(1, model.UToku);
        Assert.Equal(1, model.Zavrsene);
        Assert.Equal(1, model.Napustene);
        Assert.Equal(2, model.Omiljene);
        Assert.Equal(55, model.UkupnoSati);
        Assert.Equal(22.0 / 3, model.ProsecnaOcena!.Value, 8);
        Assert.Equal("Avantura", model.NajcesciZanr);
        Assert.Equal("Konzola", model.NajcescaPlatforma);
    }

    [Fact]
    public async Task NeocenjeneIgreIVelikiBrojSati()
    {
        context.Igre.AddRange(new Igra { Naziv = "A", BrojSati = int.MaxValue },
            new Igra { Naziv = "B", BrojSati = int.MaxValue });
        await context.SaveChangesAsync();
        var rezultat = await statistika.DohvatiAsync();
        Assert.Equal(2L * int.MaxValue, rezultat.UkupnoSati);
        Assert.Null(rezultat.ProsecnaOcena);
        Assert.Null(rezultat.NajcesciZanr);
    }

    public void Dispose()
    {
        context.Dispose();
        connection.Dispose();
    }
}
