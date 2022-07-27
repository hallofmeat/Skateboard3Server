using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers;

[Route("/skate3/webkit/PS3/ErrorPages/404")]
public class PageNotFoundController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}