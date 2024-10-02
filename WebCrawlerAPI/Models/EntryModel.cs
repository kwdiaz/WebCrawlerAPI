using Microsoft.AspNetCore.Mvc;

namespace WebCrawlerAPI.Models
{
    public class EntryModel
    {
        public int Id { get; set; }
        public int Rank { get; set; }
        public string? Title { get; set; }
        public int Points { get; set; }
        public int Comments { get; set; }
        public int WordCount { get; set; }
    }
}
