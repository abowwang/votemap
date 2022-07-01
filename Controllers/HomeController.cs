using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using VoteMap.Core.Models;

namespace VoteMap.Core.Controllers
{
    public class HomeController : Controller
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public HomeController()
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Town(string year = "", string type = "")
        {
            TownView model = new TownView()
            {
                year = year,
                type = type
            };
            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
