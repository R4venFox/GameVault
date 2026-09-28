using GameVault.Business.Services;
using GameVault.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace GameVault.Web.Controllers;

public class StatistikaController : Controller
{
    private readonly IStatistikaService service;

    public StatistikaController(IStatistikaService service)
    {
        this.service = service;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
        => View(StatistikaViewModel.IzPodataka(await service.DohvatiAsync(cancellationToken)));
}
