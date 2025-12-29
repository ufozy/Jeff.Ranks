namespace Jeff.Ranks.Models
{
    public class CustomerRank : Customer
    {
        public CustomerRank(long customerId) : base(customerId)
        {

        }

        public long Rank { get; set; } = -1;

        /// <summary>
        /// The relative ranking is calculated based on the initial ranking value.
        /// </summary>
        /// <param name="seedRankNumber"></param>
        /// <returns></returns>
        public CustomerRank GetRealRank(long seedRankNumber)
        {
            return new CustomerRank(this.CustomerId) { Score = this.Score, Rank = this.Rank + seedRankNumber }; //Since the current value cannot be modified, an object must be created. This can be optimized using the flyweight pattern.
        }
    }
}
