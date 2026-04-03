using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HoThiBichNhung_2123110314.Models
{
    public class Address
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public string FullName { get; set; }

        public string Phone { get; set; }

        public string AddressLine { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string Ward { get; set; }

        public bool IsDefault { get; set; } = false;

        [ForeignKey("UserId")]
        [JsonIgnore]
        public User? User { get; set; }
    }
}