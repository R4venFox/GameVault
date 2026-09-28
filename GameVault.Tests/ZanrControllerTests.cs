using GameVault.Business.Services;
using GameVault.Data.Models;
using GameVault.Tests.Fakes;
using GameVault.Web.Controllers;
using GameVault.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace GameVault.Tests;

public class ZanrControllerTests
{
    private readonly ZanrRepositoryFake repository = new();
    private readonly ZanrController controller;

    public ZanrControllerTests()
    {
        controller = new ZanrController(new ZanrService(repository));
    }

    [Fact]
    public async Task CrudAkcijeRadePrekoServisa()
    {
        Assert.IsType<ViewResult>(controller.Create());
        Assert.IsType<RedirectToActionResult>(await controller.Create(new NazivViewModel { Naziv = "Prvi" }, default));
        var zapis = Assert.Single(repository.Zapisi);
        var index = Assert.IsType<ViewResult>(await controller.Index(default));
        Assert.Single(Assert.IsType<List<Zanr>>(index.Model));
        var edit = Assert.IsType<ViewResult>(await controller.Edit(zapis.Id, default));
        Assert.Equal("Prvi", Assert.IsType<NazivViewModel>(edit.Model).Naziv);
        Assert.IsType<RedirectToActionResult>(await controller.Edit(zapis.Id,
            new NazivViewModel { Id = zapis.Id, Naziv = "Novi" }, default));
        Assert.Equal("Novi", zapis.Naziv);
        Assert.IsType<ViewResult>(await controller.Delete(zapis.Id, default));
        Assert.Single(repository.Zapisi);
        Assert.IsType<RedirectToActionResult>(await controller.DeleteConfirmed(zapis.Id, default));
        Assert.Empty(repository.Zapisi);
    }

    [Fact]
    public async Task DuplikatiIPoslovneGreskeSePrikazuju()
    {
        repository.Zapisi.Add(new Zanr { Id = 1, Naziv = "Prvi" });
        repository.Zapisi.Add(new Zanr { Id = 2, Naziv = "Drugi" });
        Assert.IsType<ViewResult>(await controller.Create(new NazivViewModel { Naziv = " prvi " }, default));
        Assert.False(controller.ModelState.IsValid);
        controller.ModelState.Clear();
        Assert.IsType<ViewResult>(await controller.Edit(2, new NazivViewModel { Id = 2, Naziv = "PRVI" }, default));
        Assert.False(controller.ModelState.IsValid);
        controller.ModelState.Clear();
        repository.KorisceniId.Add(1);
        var delete = Assert.IsType<ViewResult>(await controller.DeleteConfirmed(1, default));
        Assert.Equal("Delete", delete.ViewName);
        Assert.False(controller.ModelState.IsValid);
        Assert.Equal(2, repository.Zapisi.Count);
    }

    [Fact]
    public async Task NepostojeciIdINeusaglasenIdSeOdbijaju()
    {
        Assert.IsType<NotFoundResult>(await controller.Edit(99, default));
        Assert.IsType<NotFoundResult>(await controller.Edit(99, new NazivViewModel { Id = 99 }, default));
        Assert.IsType<BadRequestResult>(await controller.Edit(1, new NazivViewModel { Id = 2 }, default));
        Assert.IsType<NotFoundResult>(await controller.Delete(99, default));
        Assert.IsType<NotFoundResult>(await controller.DeleteConfirmed(99, default));
    }

    [Fact]
    public async Task NevalidnaFormaNeUpisujePodatke()
    {
        repository.Zapisi.Add(new Zanr { Id = 1, Naziv = "Prvi" });
        controller.ModelState.AddModelError("Naziv", "Naziv je obavezan.");
        Assert.IsType<ViewResult>(await controller.Create(new NazivViewModel(), default));
        Assert.IsType<ViewResult>(await controller.Edit(1, new NazivViewModel { Id = 1 }, default));
        Assert.Equal("Prvi", Assert.Single(repository.Zapisi).Naziv);
    }
}
