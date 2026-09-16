using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BudgetBook.Models;
using Microsoft.AspNetCore.Authorization;

namespace BudgetBook.Controllers;

[Authorize]
public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("Home/StatusCode")]
    public IActionResult StatusCode(int code)
    {
        if (code == 404)
        {
            return View("NotFound");
        }

        ViewData["StatusCode"] = code;
        return View("Error");
    }
}
