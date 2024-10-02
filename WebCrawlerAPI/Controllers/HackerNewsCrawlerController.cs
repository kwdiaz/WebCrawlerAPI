using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebCrawlerAPI.Context;
using WebCrawlerAPI.Services;

namespace WebCrawlerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HackerNewsCrawlerController : ControllerBase
    {
        private readonly HackerNewsCrawlerService _crawler;

        public HackerNewsCrawlerController(HackerNewsContext context)
        {
            _crawler = new HackerNewsCrawlerService(context);
        }

        // GET endpoint to scrape entries from Hacker News
        [HttpGet("scrape")]
        public IActionResult Scrape()
        {
            var userIdentifier = User.FindFirst(ClaimTypes.Name)?.Value;
            _crawler.LogUsageData("Scrape", userIdentifier);

            var entries = _crawler.ScrapeEntries(); // Call the scrape method
            return Ok(entries);
        }

        // GET endpoint to filter entries with more than five words in the title
        [HttpGet("filter/morethanfivewords")]
        public IActionResult FilterMoreThanFiveWords()
        {
            var userIdentifier = User.FindFirst(ClaimTypes.Name)?.Value; 
            var filteredEntries = _crawler.FilterEntriesMoreThanFiveWords(); // Call the filter method
            _crawler.LogUsageData("MoreThanFiveWords", userIdentifier); // Log the usage data
            return Ok(filteredEntries);
        }

        // GET endpoint to filter entries with less than or equal to five words in the title
        [HttpGet("filter/lessthanfivewords")]
        public IActionResult FilterLessThanOrEqualToFiveWords()
        {
            var userIdentifier = User.FindFirst(ClaimTypes.Name)?.Value; 
            var filteredEntries = _crawler.FilterEntriesLessThanOrEqualToFiveWords(); // Call the filter method
            _crawler.LogUsageData("LessThanOrEqualToFiveWords", userIdentifier); // Log the usage data
            return Ok(filteredEntries);
        }
    }
}
