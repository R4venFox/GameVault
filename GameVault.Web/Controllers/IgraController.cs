using GameVault.Business;
using GameVault.Business.Services;
using GameVault.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameVault.Web.Controllers;

public class IgraController : Controller
{
    private readonly IIgraService igre;
    private readonly IZanrService zanrovi;
    private readonly IPlatformaService platforme;

    public IgraController(IIgraService igre, IZanrService zanrovi, IPlatformaService platforme)
    {
        this.igre = igre;
        this.zanrovi = zanrovi;
        this.platforme = platforme;
    }

    public async Task<IActionResult> Index(IgraListaViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            try
            {
                model.Igre = await igre.PretraziAsync(model.Pretraga, cancellationToken);
            }
            catch (PoslovnaGreskaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }
        }
        model.Zanrovi = (await zanrovi.DohvatiSveAsync(cancellationToken))
            .Select(z => new SelectListItem(z.Naziv, z.Id.ToString())).ToList();
        model.Platforme = (await platforme.DohvatiSveAsync(cancellationToken))
            .Select(p => new SelectListItem(p.Naziv, p.Id.ToString())).ToList();
        return View(model);
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var igra = await igre.DohvatiPoIdAsync(id, cancellationToken);
        return igra is null ? NotFound() : View(igra);
    }

    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new IgraFormaViewModel();
        await PopuniIzboreAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(IgraFormaViewModel model, CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var igra = await igre.DodajAsync(model.UPoslovnePodatke(), cancellationToken);
                return RedirectToAction(nameof(Details), new { id = igra.Id });
            }
            catch (PoslovnaGreskaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }
        }
        await PopuniIzboreAsync(model, cancellationToken);
        return View(model);
    }

    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var igra = await igre.DohvatiPoIdAsync(id, cancellationToken);
        if (igra is null)
            return NotFound();
        var model = IgraFormaViewModel.IzIgre(igra);
        await PopuniIzboreAsync(model, cancellationToken);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, IgraFormaViewModel model, CancellationToken cancellationToken)
    {
        if (id != model.Id)
            return BadRequest();
        if (await igre.DohvatiPoIdAsync(id, cancellationToken) is null)
            return NotFound();
        if (ModelState.IsValid)
        {
            try
            {
                await igre.IzmeniAsync(id, model.UPoslovnePodatke(), cancellationToken);
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (PoslovnaGreskaException exception)
            {
                ModelState.AddModelError(string.Empty, exception.Message);
            }
        }
        await PopuniIzboreAsync(model, cancellationToken);
        return View(model);
    }

    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var igra = await igre.DohvatiPoIdAsync(id, cancellationToken);
        return igra is null ? NotFound() : View(igra);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var igra = await igre.DohvatiPoIdAsync(id, cancellationToken);
        if (igra is null)
            return NotFound();
        try
        {
            await igre.ObrisiAsync(id, cancellationToken);
            return RedirectToAction(nameof(Index));
        }
        catch (PoslovnaGreskaException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            return View("Delete", igra);
        }
    }

    private async Task PopuniIzboreAsync(IgraFormaViewModel model, CancellationToken cancellationToken)
    {
        model.Zanrovi = (await zanrovi.DohvatiSveAsync(cancellationToken))
            .Select(z => new SelectListItem(z.Naziv, z.Id.ToString())).ToList();
        model.Platforme = (await platforme.DohvatiSveAsync(cancellationToken))
            .Select(p => new SelectListItem(p.Naziv, p.Id.ToString())).ToList();
    }
}
