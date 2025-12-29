namespace Jeff.Ranks.Models
{
    public class Customer
    {
        public long CustomerId { get; }
        public decimal Score { get; set; } = 0;

        public Customer(long customerId)
        {
            this.CustomerId = customerId;
        }
    }
}