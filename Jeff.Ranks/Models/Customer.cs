using System.Text.Json.Serialization;

namespace Jeff.Ranks.Models
{
    public class Customer
    {
        [JsonPropertyName("Customer ID")]
        public long CustomerId { get; }
        [JsonPropertyName("Score")]
        public decimal Score { get; set; } = 0;

        public Customer(long customerId)
        {
            this.CustomerId = customerId;
        }
    }
}