using Microsoft.AspNetCore.Mvc;

namespace Skateboard3Server.Web.Controllers;

//TODO: not sure if personaid or userid
[Route("/skate3/webkit/PS3/English/i/Users")]
public class UsersController : Controller
{
    [HttpGet("Show/{userId}")]
    public IActionResult Show(string userId)
    {
        return View();
    }

    [HttpGet("Invitations/{userId}")]
    public IActionResult Invitations(string userId)
    {
        return View();
    }
}