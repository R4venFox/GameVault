using Microsoft.AspNetCore.Mvc;

namespace GameVault.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();
}
