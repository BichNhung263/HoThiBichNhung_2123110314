using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HoThiBichNhung_2123110314.Models
{
    public class Payment
    {
        public long Id { get; set; }

        public long OrderId { get; set; }

        public string Method { get; set; } 

        public string Status { get; set; } = "Pending";

        public DateTime PaidAt { get; set; } = DateTime.Now;

        [ForeignKey("OrderId")]
        [JsonIgnore]
        public Order? Order { get; set; }
    }
}