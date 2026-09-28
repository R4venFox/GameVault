using GameVault.Data;
using GameVault.Data.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Xunit;

namespace GameVault.Tests;

public class DatabaseTests : IDisposable
{
    private readonly SqliteConnection connection;
    private readonly GameVaultDbContext context;

    public DatabaseTests()
    {
        connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=True");
        connection.Open();
        var options = new DbContextOptionsBuilder<GameVaultDbContext>()
            .UseSqlite(connection)
            .Options;
        context = new GameVaultDbContext(options);
        context.Database.Migrate();
    }

    [Fact]
    public async Task Migracija_KreiraPraznuBazuBezNeprimenjenihPromena()
    {
        Assert.Equal(2, (await context.Database.GetAppliedMigrationsAsync()).Count());
        Assert.Empty(await context.Database.GetPendingMigrationsAsync());
        Assert.False(context.Database.HasPendingModelChanges());
        Assert.Empty(await context.Igre.ToListAsync());
        Assert.Empty(await context.Zanrovi.ToListAsync());
        Assert.Empty(await context.Platforme.ToListAsync());
    }

    [Fact]
    public async Task Veze_ViseIgaraDeliZanroveIPlatforme()
    {
        var zanr = new Zanr { Naziv = "Avantura" };
        var platforma = new Platforma { Naziv = "PC" };
        context.Igre.AddRange(
            new Igra
            {
                Naziv = "Prva igra",
                Zanrovi = new List<Zanr> { zanr, new() { Naziv = "Logicka" } },
                Platforme = new List<Platforma> { platforma, new() { Naziv = "PlayStation" } }
            },
            new Igra
            {
                Naziv = "Druga igra",
                Zanrovi = new List<Zanr> { zanr },
                Platforme = new List<Platforma> { platforma }
            });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var igra = await context.Igre.Include(i => i.Zanrovi).Include(i => i.Platforme)
            .SingleAsync(i => i.Naziv == "Prva igra");
        Assert.Equal(2, igra.Zanrovi.Count);
        Assert.Equal(2, igra.Platforme.Count);
        Assert.Equal(2, await context.Zanrovi.Where(z => z.Naziv == "Avantura")
            .SelectMany(z => z.Igre).CountAsync());
        Assert.Equal(2, await context.Platforme.Where(p => p.Naziv == "PC")
            .SelectMany(p => p.Igre).CountAsync());

        context.Igre.Remove(igra);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        Assert.Equal(2, await context.Zanrovi.CountAsync());
        Assert.Equal(2, await context.Platforme.CountAsync());
        Assert.Single(await context.Igre.ToListAsync());
        Assert.Equal(1, await context.Database.SqlQueryRaw<int>(
            "SELECT COUNT(*) AS Value FROM IgraZanr").SingleAsync());
        Assert.Equal(1, await context.Database.SqlQueryRaw<int>(
            "SELECT COUNT(*) AS Value FROM IgraPlatforma").SingleAsync());
    }

    [Theory]
    [InlineData("Igra", null)]
    [InlineData("Igra", "")]
    [InlineData("Igra", "   ")]
    [InlineData("Zanr", null)]
    [InlineData("Zanr", "")]
    [InlineData("Zanr", "   ")]
    [InlineData("Platforma", null)]
    [InlineData("Platforma", "")]
    [InlineData("Platforma", "   ")]
    public async Task Baza_OdbijaNedostajuciNaziv(string model, string? naziv)
    {
        object entitet = model switch
        {
            "Igra" => new Igra { Naziv = naziv! },
            "Zanr" => new Zanr { Naziv = naziv! },
            _ => new Platforma { Naziv = naziv! }
        };
        context.Add(entitet);
        await OcekivanoOgranicenje();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Baza_OdbijaDuplikateNaziva(bool zanr)
    {
        if (zanr)
            context.Zanrovi.AddRange(new Zanr { Naziv = "RPG" }, new Zanr { Naziv = "RPG" });
        else
            context.Platforme.AddRange(new Platforma { Naziv = "PC" }, new Platforma { Naziv = "PC" });

        await OcekivanoOgranicenje();
    }

    [Theory]
    [InlineData(0, 0, StatusIgre.Planirana)]
    [InlineData(11, 0, StatusIgre.Planirana)]
    [InlineData(null, -1, StatusIgre.Planirana)]
    [InlineData(null, 0, (StatusIgre)99)]
    public async Task Baza_OdbijaNeispravnuOcenuSateIStatus(int? ocena, int sati, StatusIgre status)
    {
        context.Igre.Add(new Igra { Naziv = "Portal", Ocena = ocena, BrojSati = sati, Status = status });
        await OcekivanoOgranicenje();
    }

    [Theory]
    [InlineData(null, 0, StatusIgre.Planirana)]
    [InlineData(1, 1, StatusIgre.UToku)]
    [InlineData(10, 20, StatusIgre.Zavrsena)]
    [InlineData(5, 3, StatusIgre.Napustena)]
    public async Task Baza_CuvaDozvoljeneVrednosti(int? ocena, int sati, StatusIgre status)
    {
        context.Igre.Add(new Igra { Naziv = "Portal", Ocena = ocena, BrojSati = sati, Status = status });
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var igra = await context.Igre.SingleAsync();
        Assert.Equal(ocena, igra.Ocena);
        Assert.Equal(sati, igra.BrojSati);
        Assert.Equal(status, igra.Status);
    }

    [Fact]
    public async Task Migracija_MozeDaSePonistiIPonovoPrimeni()
    {
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync(Migration.InitialDatabase);
        Assert.Empty(await context.Database.GetAppliedMigrationsAsync());
        await migrator.MigrateAsync();
        Assert.Equal(2, (await context.Database.GetAppliedMigrationsAsync()).Count());
        Assert.Empty(await context.Igre.ToListAsync());
    }

    [Fact]
    public async Task MigracijaBeleski_CuvaPostojecePodatke()
    {
        var migrator = context.GetService<IMigrator>();
        await migrator.MigrateAsync("20260928142024_PocetnaBaza");
        await context.Database.ExecuteSqlRawAsync("""
            INSERT INTO Igre (Naziv, Opis, Status, BrojSati, Omiljena, DatumDodavanja)
            VALUES ('Stara igra', 'Opis igre', 0, 0, 0, '2026-01-01 00:00:00');
            """);
        await migrator.MigrateAsync();
        var igra = await context.Igre.SingleAsync();
        Assert.Equal("Stara igra", igra.Naziv);
        Assert.Equal("Opis igre", igra.Opis);
        Assert.Null(igra.Beleske);
        igra.Beleske = "Licna napomena\nDrugi red";
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        Assert.Equal("Licna napomena\nDrugi red", (await context.Igre.SingleAsync()).Beleske);
        await migrator.MigrateAsync("20260928142024_PocetnaBaza");
        context.ChangeTracker.Clear();
        await migrator.MigrateAsync();
        Assert.Null((await context.Igre.SingleAsync()).Beleske);
        Assert.Equal("Opis igre", (await context.Igre.SingleAsync()).Opis);
    }

    private async Task OcekivanoOgranicenje()
    {
        var exception = await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync());
        var sqliteException = Assert.IsType<SqliteException>(exception.InnerException);
        Assert.Equal(19, sqliteException.SqliteErrorCode);
    }

    public void Dispose()
    {
        context.Dispose();
        connection.Dispose();
    }
}
