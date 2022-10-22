using Microsoft.AspNetCore.Mvc;

namespace Portal.Controllers;

public class AccountController : Controller
{
    public IActionResult Index()
    {
        return View("MyReservations"); 
    }

    public IActionResult Login()
    {
        return View();
    }
}

