using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Branch
    {
        [JsonIgnore]
        [Key]
        public long Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Book> AvailableBooks { get; set; }
        public List<Book> RentedBooks { get; set; }
    }
}