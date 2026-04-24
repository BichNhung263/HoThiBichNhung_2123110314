using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HoThiBichNhung_2123110314.Models
{
    public class Order
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;



        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        [JsonIgnore]
        public User? User { get; set; }

        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}