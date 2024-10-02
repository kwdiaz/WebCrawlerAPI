using System.ComponentModel.DataAnnotations;

namespace WebCrawlerAPI.Models
{
    public class UserModel
    {
        [Key]
        public int Id { get; private set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
    }

}
