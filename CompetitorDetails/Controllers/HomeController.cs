using CompetitorDetails.Data;
using CompetitorDetails.Models;
using CompetitorDetails.Selenium;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
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
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetArticleData()
        {
            List<ArticleDetail> articleslist = new List<ArticleDetail>();
            List<ArticleDetail>? articles = await _context.articles.ToListAsync();
            if (articles.Count > 0)
            {
                articleslist = SearchArticles(articles);
            }
            return Json(articleslist);
        }

        private List<ArticleDetail> SearchArticles(List<ArticleDetail> articleDetails)
        {
            var articles = articleDetails.Select(article =>
            {
                var timeDifference = DateTime.Now - DateTime.Parse(article.ArticleTime);
                string times = GetTimeElapsedString(timeDifference);
                return new ArticleDetail
                {
                    ArticleTitle = article.ArticleTitle,
                    ArticleUrl = article.ArticleUrl,
                    ArticleTime = article.ArticleTime,
                };

            }).ToList();
            return articles;
            //return PartialView("_ArticleDetail", articles);
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
        // Function to convert TimeSpan to human-readable string
        static string GetTimeElapsedString(TimeSpan timeSpan)
        {
            int totalDays = (int)timeSpan.TotalDays;
            
            return totalDays.ToString();
            //if (timeSpan.TotalDays >= 365)
            //{
            //    int years = (int)(timeSpan.TotalDays / 365);
            //    return years == 1 ? "1 year ago" : years + " years ago";
            //}
            //else if (timeSpan.TotalDays >= 30)
            //{
            //    int months = (int)(timeSpan.TotalDays / 30);
            //    return months == 1 ? "1 month ago" : months + " months ago";
            //}
            //else if (timeSpan.TotalDays >= 7)
            //{
            //    int weeks = (int)(timeSpan.TotalDays / 7);
            //    return weeks == 1 ? "1 week ago" : weeks + " weeks ago";
            //}
            //else if (timeSpan.TotalDays >= 1)
            //{
            //    int days = (int)timeSpan.TotalDays;
            //    return days == 1 ? "1 day ago" : days + " days ago";
            //}
            //else if (timeSpan.TotalHours >= 1)
            //{
            //    int hours = (int)timeSpan.TotalHours;
            //    return hours == 1 ? "1 hour ago" : hours + " hours ago";
            //}
            //else if (timeSpan.TotalMinutes >= 1)
            //{
            //    int minutes = (int)timeSpan.TotalMinutes;
            //    return minutes == 1 ? "1 minute ago" : minutes + " minutes ago";
            //}
            //else
            //{
            //    return "Just now";
            //}
        }
    }
}