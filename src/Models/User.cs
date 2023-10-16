using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class User
    {
        [JsonIgnore]
        [Key]
        public long Id { get; set; }
        public String Email { get; set; }
        [PasswordPropertyText]
        public String Password { get; set; }
        public DateTimeOffset DateRegistred { get; set; }

        public List<Book> RentedBooks { get; set; }

        // security
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }

    }
}