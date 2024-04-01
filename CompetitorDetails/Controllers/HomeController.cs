using CompetitorDetails.Data;
using CompetitorDetails.Models;
using CompetitorDetails.Selenium;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CompetitorDetails.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CompetitorArticle competitorArticle = new CompetitorArticle();

        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string vsfssd = string.Empty;
            List<ArticleDetail> articles = await _context.articles.ToListAsync();
            SearchArticles(articles);
            return View();
        }
        private IActionResult SearchArticles(List<ArticleDetail> articleDetails)
        {
            List<ArticleDetail>? todayData = articleDetails?
                .Where(d => d.ArticleTime.Contains("h"))
                .DistinctBy(a => a.ArticleTitle)
                .ToList();
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