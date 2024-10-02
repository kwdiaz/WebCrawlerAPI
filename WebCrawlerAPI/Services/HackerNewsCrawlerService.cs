using HtmlAgilityPack;
using WebCrawlerAPI.Context;
using WebCrawlerAPI.Models;
using System.Text.RegularExpressions;
using System.Net;

namespace WebCrawlerAPI.Services
{
    public class HackerNewsCrawlerService
    {
     
        private readonly HttpClient _httpClient;
        private readonly HackerNewsContext _context;

        public HackerNewsCrawlerService(HackerNewsContext context)
        {
            _httpClient = new HttpClient();
            _context = context;
        }

        // Method to scrape entries from Hacker News
        public List<EntryModel> ScrapeEntries()
        {
            // Load the Hacker News page
            var web = new HtmlWeb();
            var doc = web.Load("https://news.ycombinator.com/");

            // Select the top 30 entries
            var entries = doc.DocumentNode.SelectNodes("//tr[@class='athing']")
                ?.Take(30)
                .Select(node =>
                {
                    // Extract rank from the entry
                    var rankNode = node.SelectSingleNode("td[@class='title']/span[@class='rank']");
                    var rank = rankNode != null ? int.Parse(rankNode.InnerText.TrimEnd('.')) : 0; // Remove the trailing dot

                    // Extract title of the entry
                    var titleNode = node.SelectSingleNode("td[@class='title']/span[@class='titleline']/a");
                    var title = titleNode != null ? titleNode.InnerText.Trim() : "No Title"; // Handle potential null

                    // Count words in the title
                    var wordCount = CountWords(title);

                    // Find the subtext node containing points and comments
                    var subtextNode = FindSubtextNode(node);

                    var points = 0;
                    var comments = 0;

                    if (subtextNode != null)
                    {
                        // Extract points from subtext
                        var pointsNode = subtextNode.SelectSingleNode(".//span[@class='score']");
                        if (pointsNode != null)
                        {
                            var pointsText = pointsNode.InnerText.Trim();
                            if (pointsText.EndsWith("points"))
                            {
                                pointsText = pointsText.Replace("points", "").Trim();
                            }
                            points = int.TryParse(pointsText, out int parsedPoints) ? parsedPoints : 0;
                        }

                        // Extract comments count from subtext
                        var commentsNode = subtextNode.SelectNodes(".//a[contains(text(), 'comment')]");
                        if (commentsNode != null && commentsNode.Count > 0)
                        {
                            var commentsText = commentsNode.Last().InnerText.Trim();
                            commentsText = commentsText.Replace("&nbsp;", " "); // Replace non-breaking space with a normal space

                            if (commentsText.EndsWith("comments"))
                            {
                                commentsText = commentsText.Replace("comments", "").Trim();
                            }

                            comments = int.TryParse(commentsText, out int parsedComments) ? parsedComments : 0;
                        }
                    }

                    // Capture the creation time (set to current time for demo)
                    var createdAt = DateTime.UtcNow; // Use current time for demo purposes

                    return new EntryModel { Rank = rank, Title = title, Points = points, Comments = comments, WordCount = wordCount }; // Return the entry model
                }).ToList() ?? new List<EntryModel>(); // Handle potential null from SelectNodes

            // Save entries to the database
            _context.Entries.AddRange(entries);
            _context.SaveChanges();

            return entries;
        }

        // Method to find the subtext node (points and comments)
        private HtmlNode FindSubtextNode(HtmlNode currentNode)
        {
            var sibling = currentNode.NextSibling;

            // Iterate through siblings to find the one with the subtext class
            while (sibling != null)
            {
                if (sibling.Name == "tr" && sibling.SelectSingleNode("td[@class='subtext']") != null)
                {
                    return sibling.SelectSingleNode("td[@class='subtext']");
                }

                sibling = sibling.NextSibling; // Move to the next sibling
            }

            return null; // Return null if no subtext node is found
        }

        // Method to count the number of words in a title
        private int CountWords(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return 0;

            // Decode HTML entities
            title = WebUtility.HtmlDecode(title);

            // Remove unwanted characters but keep apostrophes
            title = Regex.Replace(title, @"[^\w\s']", " ");

            // Replace multiple spaces with a single space
            title = Regex.Replace(title.Trim(), @"\s+", " ");

            // Split the title into words and return the count
            var words = title.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Length;
        }

        // Method to filter entries with more than five words in the title
        public List<EntryModel> FilterEntriesMoreThanFiveWords()
        {
            var filteredEntries = _context.Entries
                .AsEnumerable()
                .Where(e => CountWords(e.Title) > 5)
                .OrderByDescending(e => e.Comments)
                .ToList();

            return filteredEntries;
        }

        // Method to filter entries with less than or equal to five words in the title
        public List<EntryModel> FilterEntriesLessThanOrEqualToFiveWords()
        {
            var filteredEntries = _context.Entries
                .AsEnumerable()
                .Where(e => CountWords(e.Title) <= 5)
                .OrderByDescending(e => e.Points)
                .ToList();


            return filteredEntries;
        }

        // Method to log usage data
        public void LogUsageData(string filter, string userIdentifier = null)
        {

            var usageData = new UsageDataModel
            {
                RequestTimestamp = DateTime.UtcNow,
                AppliedFilter = filter,
                UserIdentifier = userIdentifier, 
               
            };
            _context.UsageData.Add(usageData);
            _context.SaveChanges();
        }

    }
}
