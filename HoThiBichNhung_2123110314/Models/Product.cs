using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HoThiBichNhung_2123110314.Models
{
    public class Product
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")] 
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public int Quantity { get; set; }

   
        public long CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [JsonIgnore] 
        public Category? Category { get; set; }

        [JsonIgnore] 
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}