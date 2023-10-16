using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models
{
    public class Book
    {
        [Key]
        public long Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }
}