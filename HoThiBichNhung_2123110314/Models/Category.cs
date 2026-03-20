using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HoThiBichNhung_2123110314.Models
{
    public class Category
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string? Description { get; set; }

        [JsonIgnore] 
        public ICollection<Product>? Products { get; set; }
    }
}