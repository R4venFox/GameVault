using System.ComponentModel.DataAnnotations;
using GameVault.Data.Models;
using Xunit;

namespace GameVault.Tests;

public class ModelTests
{
    [Theory]
    [InlineData(null)]
    [InlineData(1)]
    [InlineData(10)]
    public void Igra_PrihvataOpcionuOcenuIGranicneVrednosti(int? ocena)
    {
        Assert.True(Validan(new Igra { Naziv = "Portal", Ocena = ocena }));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    public void Igra_OdbijaOcenuVanRaspona(int ocena)
    {
        Assert.False(Validan(new Igra { Naziv = "Portal", Ocena = ocena }));
    }

    [Fact]
    public void Igra_OdbijaNegativneSateINepostojeciStatus()
    {
        Assert.False(Validan(new Igra { Naziv = "Portal", BrojSati = -1 }));
        Assert.False(Validan(new Igra { Naziv = "Portal", Status = (StatusIgre)99 }));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Modeli_ZahtevajuNaziv(string? naziv)
    {
        Assert.False(Validan(new Igra { Naziv = naziv! }));
        Assert.False(Validan(new Zanr { Naziv = naziv! }));
        Assert.False(Validan(new Platforma { Naziv = naziv! }));
    }

    [Fact]
    public void NovaIgra_ImaPocetneVrednosti()
    {
        var preKreiranja = DateTime.UtcNow;
        var igra = new Igra { Naziv = "Portal" };

        Assert.Equal(StatusIgre.Planirana, igra.Status);
        Assert.Null(igra.Ocena);
        Assert.Equal(0, igra.BrojSati);
        Assert.False(igra.Omiljena);
        Assert.Empty(igra.Zanrovi);
        Assert.Empty(igra.Platforme);
        Assert.Equal(DateTimeKind.Utc, igra.DatumDodavanja.Kind);
        Assert.InRange(igra.DatumDodavanja, preKreiranja, DateTime.UtcNow);
    }

    private static bool Validan(object model)
    {
        return Validator.TryValidateObject(model, new ValidationContext(model),
            new List<ValidationResult>(), validateAllProperties: true);
    }
}
