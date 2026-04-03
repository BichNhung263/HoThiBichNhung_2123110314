using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HoThiBichNhung_2123110314.Models
{
    public class CartItem
    {
        public long Id { get; set; }

        public long CartId { get; set; }

        public long ProductId { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("CartId")]
        [JsonIgnore]
        public Cart? Cart { get; set; }

        [ForeignKey("ProductId")]
        [JsonIgnore]
        public Product? Product { get; set; }
    }
}