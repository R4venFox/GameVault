using GameVault.Business;
using GameVault.Business.Services;
using GameVault.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Web.Controllers;

public class ZanrController : Controller
{
    private readonly IZanrService service;

    public ZanrController(IZanrService service)
    {
        this.service = service;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
        => View(await service.DohvatiSveAsync(cancellationToken));

    public IActionResult Create() => View(new NazivViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NazivViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await service.DodajAsync(model.Naziv, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            catch (PoslovnaGreskaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var zapis = await service.DohvatiPoIdAsync(id, cancellationToken);
        return zapis is null ? NotFound() : View(new NazivViewModel { Id = zapis.Id, Naziv = zapis.Naziv });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, NazivViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest();
        if (await service.DohvatiPoIdAsync(id, cancellationToken) is null)
            return NotFound();
        if (ModelState.IsValid)
        {
            try
            {
                await service.IzmeniAsync(id, model.Naziv, cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            catch (PoslovnaGreskaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }
        }
        return View(model);
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var zapis = await service.DohvatiPoIdAsync(id, cancellationToken);
        return zapis is null ? NotFound() : View(new NazivViewModel { Id = zapis.Id, Naziv = zapis.Naziv });
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var zapis = await service.DohvatiPoIdAsync(id, cancellationToken);
        if (zapis is null)
            return NotFound();
        try
        {
            await service.ObrisiAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (PoslovnaGreskaException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("Delete", new NazivViewModel { Id = zapis.Id, Naziv = zapis.Naziv });
        }
    }
}
