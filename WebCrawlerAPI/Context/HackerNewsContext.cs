using Microsoft.EntityFrameworkCore;
using WebCrawlerAPI.Models;

namespace WebCrawlerAPI.Context
{
    public class HackerNewsContext : DbContext
    {
        public HackerNewsContext(DbContextOptions<HackerNewsContext> options)
            : base(options) { }

        public DbSet<EntryModel> Entries { get; set; }
        public DbSet<UsageDataModel> UsageData { get; set; }
    }
}
