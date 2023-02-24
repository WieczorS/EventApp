using Microsoft.AspNetCore.Mvc;

namespace EventApp.Controllers;

public class EventController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}