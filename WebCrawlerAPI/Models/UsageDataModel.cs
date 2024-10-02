namespace WebCrawlerAPI.Models
{
    public class UsageDataModel
    {
        public int Id { get; set; }
        public DateTime RequestTimestamp { get; set; }
        public string? AppliedFilter { get; set; }
    }
}
