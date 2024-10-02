using Microsoft.AspNetCore.Mvc;

namespace WebCrawlerAPI.Services
{
    public class HackerNewsCrawlerService : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
