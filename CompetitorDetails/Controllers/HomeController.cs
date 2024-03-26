using CompetitorDetails.Models;
using CompetitorDetails.Selenium;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CompetitorDetails.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CompetitorArticle competitorArticle = new CompetitorArticle();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Search(string searchTerm)
        {
            List<ArticleDetail>? articleDetailList = competitorArticle.TestGoogleSearch(searchTerm);

            List<ArticleDetail>? todayData = articleDetailList?.Where(d => d.ArticleTime.Contains("h")).ToList();
            return PartialView("_ArticleDetail", todayData);
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