using CompetitorDetails.Data;
using CompetitorDetails.Models;
using CompetitorDetails.Selenium;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace CompetitorDetails.Controllers
{
    [Authorize]
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
            // Retrieve list of drivers and cars from the database
            var brands = _context.competitors.ToList();

            // Populate ViewBag with SelectList for DriverId and CarId
            ViewBag.BrandId = new SelectList(brands, "Id", "Name"); // Assuming Driver has an "Id" and "Name" property
            return View();
        }
        public async Task<IActionResult> GetArticleData(Competitor competitor)
        {
            List<ArticleViewModel> articleslist = new List<ArticleViewModel>();
            var articles = await _context.articles.Where(id => id.BrandId == competitor.Id)
                .Include(c => c.Competitors)
                .ToListAsync();
            if (articles.Count > 0)
            {
                foreach (var item in articles)
                {
                    var article = new ArticleViewModel
                    {
                        ArticleTitle = item.ArticleTitle,
                        ArticleTime = item.ArticleTime,
                        ArticleUrl = item.ArticleUrl,
                        BrandName = item.Competitors?.Name,
                    };
                    articleslist.Add(article);
                }
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
        public IActionResult Competitors()
        {
            return View();
        }
        public async Task<IActionResult> GetCompetitorsData()
        {
            //List<Competitor> competitorlist = new List<Competitor>();
            List<Competitor>? competitors = await _context.competitors.ToListAsync();
            
            return Json(competitors);
        }
        [HttpPost]
        public async Task<IActionResult> AddCompetitor([FromBody] Competitor competitor)
        {
            bool alreadyCheck = _context.competitors.Any(u => u.Url == competitor.Url);
            if (!alreadyCheck)
            {
                var newBrand = new Competitor
                {
                    Id = Guid.NewGuid(),
                    Name = competitor.Name,
                    Url = competitor.Url,
                    Status = competitor.Status,
                };
                await _context.competitors.AddAsync(newBrand);
                await _context.SaveChangesAsync();
                return Json(new { message = "Success" });
            }
            // Handle form data here
            // You can access the submitted data via the 'data' parameter
            return Json(new { message = "Something Wrong please check try again" });
        }
        [HttpPost]
        public async Task<IActionResult> DeteleCompetitor(Guid competitorId)
        {
            var alreadyCheck = _context.competitors.FirstOrDefault(i => i.Id == competitorId);
            if (alreadyCheck != null)
            {
                _context.competitors.Remove(alreadyCheck);
                await _context.SaveChangesAsync();
                return Json(new { message = "Success" });
            }
            // Handle form data here
            // You can access the submitted data via the 'data' parameter
            return Json(new { message = "Something Wrong please check try again" });
        }

        [HttpPost]
        public IActionResult SubmitForm(Competitor model)
        {
            if (model != null)
            {
                // Your logic here to handle the form submission
                // For example, you could save the data to the database
                return Json(new { success = true, message = "Form submitted successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Failed to submit form. Model is null." });
            }
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