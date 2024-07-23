using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
       // private readonly HomeRepositories _homeRepositories;

        public HomeController(ILogger<HomeController> logger)
        {

            _logger = logger;
              
        }

        public IActionResult Index()
        {
            HomeRepositories homeRepositories = new HomeRepositories();
            homeRepositories.CallPDF();
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
    }
}
