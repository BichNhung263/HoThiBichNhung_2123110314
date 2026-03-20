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

        [Column(TypeName = "decimal(18,2)")] // 🔥 FIX WARNING DECIMAL
        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? Image { get; set; }

        public int Quantity { get; set; }

        // FK
        public long CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        [JsonIgnore] // 🔥 TRÁNH LOOP + LỖI POST
        public Category? Category { get; set; }

        [JsonIgnore] // 🔥 TRÁNH LOOP
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}