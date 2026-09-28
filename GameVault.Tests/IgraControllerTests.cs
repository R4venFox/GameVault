using GameVault.Business.Services;
using GameVault.Data.Models;
using GameVault.Tests.Fakes;
using GameVault.Web.Controllers;
using GameVault.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GameVault.Tests;

public class IgraControllerTests
{
    [Fact]
    public async Task Index_NevalidniKriterijumiPrikazujuGreskuIIzbore()
    {
        controller.ModelState.AddModelError("Pretraga.Sortiranje", "Uneta vrednost nije ispravna.");
        var model = new IgraListaViewModel();
        var result = Assert.IsType<ViewResult>(await controller.Index(model, default));
        Assert.Same(model, result.Model);
        Assert.Null(service.PoslednjaPretraga);
        Assert.Single(model.Zanrovi);
        Assert.Single(model.Platforme);
    }

    private readonly IgraServiceFake service = new();
    private readonly IgraController controller;

    public IgraControllerTests()
    {
        var zanrovi = new ZanrRepositoryFake();
        zanrovi.Zapisi.Add(new Zanr { Id = 1, Naziv = "Logicka" });
        var platforme = new PlatformaRepositoryFake();
        platforme.Zapisi.Add(new Platforma { Id = 1, Naziv = "PC" });
        controller = new IgraController(service, new ZanrService(zanrovi), new PlatformaService(platforme));
    }

    [Fact]
    public async Task Index_VracaListuIgara()
    {
        var model = new IgraListaViewModel();
        model.Pretraga.Tekst = "Portal";
        var result = Assert.IsType<ViewResult>(await controller.Index(model, default));
        Assert.Same(service.Igra, Assert.Single(Assert.IsType<IgraListaViewModel>(result.Model).Igre));
        Assert.Same(model.Pretraga, service.PoslednjaPretraga);
        Assert.Single(model.Zanrovi);
        Assert.Single(model.Platforme);
    }

    [Fact]
    public async Task Details_VracaPostojecuIgru()
    {
        var result = Assert.IsType<ViewResult>(await controller.Details(1, default));
        Assert.Same(service.Igra, result.Model);
    }

    [Fact]
    public async Task NepostojeciId_Vraca404()
    {
        Assert.IsType<NotFoundResult>(await controller.Details(99, default));
        Assert.IsType<NotFoundResult>(await controller.Edit(99, default));
        Assert.IsType<NotFoundResult>(await controller.Edit(99, new IgraFormaViewModel { Id = 99 }, default));
        Assert.IsType<NotFoundResult>(await controller.Delete(99, default));
        Assert.IsType<NotFoundResult>(await controller.DeleteConfirmed(99, default));
        Assert.Null(service.ObrisanId);
    }

    [Fact]
    public async Task Create_PripremaIzbore()
    {
        var result = Assert.IsType<ViewResult>(await controller.Create(default));
        var model = Assert.IsType<IgraFormaViewModel>(result.Model);
        Assert.Equal("Logicka", Assert.Single(model.Zanrovi).Text);
        Assert.Equal("PC", Assert.Single(model.Platforme).Text);
    }

    [Fact]
    public async Task Create_ProsledjujeFormuIPreusmeravaNaDetalje()
    {
        var model = new IgraFormaViewModel
        {
            Naziv = "Portal 2", Opis = "Opis", Beleske = "Moje beleske", GodinaIzdanja = 2011,
            Developer = "Valve", Izdavac = "Valve", Status = StatusIgre.UToku,
            Ocena = 9, BrojSati = 12, Omiljena = true,
            ZanrIds = new() { 1 }, PlatformaIds = new() { 1 }
        };
        var result = Assert.IsType<RedirectToActionResult>(await controller.Create(model, default));
        Assert.Equal("Details", result.ActionName);
        Assert.Equal(2, result.RouteValues!["id"]);
        var podaci = service.SacuvaniPodaci!;
        Assert.Equal(model.Naziv, podaci.Naziv);
        Assert.Equal(model.Opis, podaci.Opis);
        Assert.Equal(model.Beleske, podaci.Beleske);
        Assert.Equal(model.GodinaIzdanja, podaci.GodinaIzdanja);
        Assert.Equal(model.Developer, podaci.Developer);
        Assert.Equal(model.Izdavac, podaci.Izdavac);
        Assert.Equal(model.Status, podaci.Status);
        Assert.Equal(model.Ocena, podaci.Ocena);
        Assert.Equal(model.BrojSati, podaci.BrojSati);
        Assert.True(podaci.Omiljena);
        Assert.Equal(model.ZanrIds, podaci.ZanrIds);
        Assert.Equal(model.PlatformaIds, podaci.PlatformaIds);
    }

    [Fact]
    public async Task Create_PoslovnaGreskaCuvaUnosIIzbore()
    {
        service.Greska = "Godina izdanja nije ispravna.";
        var model = new IgraFormaViewModel { Naziv = "Portal", ZanrIds = new() { 1 }, PlatformaIds = new() { 1 } };
        var result = Assert.IsType<ViewResult>(await controller.Create(model, default));
        Assert.Same(model, result.Model);
        Assert.Equal(service.Greska, Assert.Single(controller.ModelState[string.Empty]!.Errors).ErrorMessage);
        Assert.Single(model.Zanrovi);
        Assert.Single(model.Platforme);
        Assert.Equal(1, Assert.Single(model.ZanrIds));
        Assert.Equal(1, Assert.Single(model.PlatformaIds));
        Assert.Null(service.SacuvaniPodaci);
    }

    [Fact]
    public async Task NevalidanModel_NePozivaUpis()
    {
        controller.ModelState.AddModelError("Naziv", "Naziv je obavezan.");
        var model = new IgraFormaViewModel { Id = 1 };
        Assert.IsType<ViewResult>(await controller.Create(model, default));
        Assert.IsType<ViewResult>(await controller.Edit(1, model, default));
        Assert.Null(service.SacuvaniPodaci);
        Assert.Single(model.Zanrovi);
        Assert.Single(model.Platforme);
    }

    [Fact]
    public async Task Edit_UcitavaOdabraneVeze()
    {
        service.Igra!.Zanrovi.Add(new Zanr { Id = 1 });
        service.Igra.Platforme.Add(new Platforma { Id = 1 });
        var result = Assert.IsType<ViewResult>(await controller.Edit(1, default));
        var model = Assert.IsType<IgraFormaViewModel>(result.Model);
        Assert.Equal("Portal", model.Naziv);
        Assert.Equal(1, Assert.Single(model.ZanrIds));
        Assert.Equal(1, Assert.Single(model.PlatformaIds));
    }

    [Fact]
    public async Task Edit_CuvaIzmene()
    {
        var result = Assert.IsType<RedirectToActionResult>(await controller.Edit(1,
            new IgraFormaViewModel { Id = 1, Naziv = "Novi naziv" }, default));
        Assert.Equal(1, service.IzmenjenId);
        Assert.Equal("Novi naziv", service.SacuvaniPodaci!.Naziv);
        Assert.Equal("Details", result.ActionName);
        Assert.Equal(1, result.RouteValues!["id"]);
    }

    [Fact]
    public async Task Edit_OdbijaRazliciteIdVrednosti()
    {
        Assert.IsType<BadRequestResult>(await controller.Edit(1, new IgraFormaViewModel { Id = 2 }, default));
        Assert.Null(service.IzmenjenId);
    }

    [Fact]
    public async Task Edit_PoslovnaGreskaVracaFormu()
    {
        service.Greska = "Žanr ne postoji.";
        var model = new IgraFormaViewModel { Id = 1, Naziv = "Portal" };
        var result = Assert.IsType<ViewResult>(await controller.Edit(1, model, default));
        Assert.Same(model, result.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.Single(model.Zanrovi);
        Assert.Single(model.Platforme);
    }

    [Fact]
    public async Task Delete_GetNeBriseAPostBrise()
    {
        var view = Assert.IsType<ViewResult>(await controller.Delete(1, default));
        Assert.Same(service.Igra, view.Model);
        Assert.Null(service.ObrisanId);
        var result = Assert.IsType<RedirectToActionResult>(await controller.DeleteConfirmed(1, default));
        Assert.Equal(1, service.ObrisanId);
        Assert.Equal("Index", result.ActionName);
    }

    [Fact]
    public async Task Delete_PoslovnaGreskaPrikazujePoruku()
    {
        service.Greska = "Igra ne postoji.";
        var result = Assert.IsType<ViewResult>(await controller.DeleteConfirmed(1, default));
        Assert.Equal("Delete", result.ViewName);
        Assert.Same(service.Igra, result.Model);
        Assert.False(controller.ModelState.IsValid);
        Assert.Null(service.ObrisanId);
    }
}
