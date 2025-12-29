namespace Jeff.Ranks.Models
{
    /// <summary>
    /// Cache Groups
    /// This is a doubly linked list.
    /// </summary>
    public class CustomerRankGroup
    {
        /// <summary>
        /// Data set.
        /// </summary>
        public List<CustomerRank> List { get; set; } = new List<CustomerRank>();
        /// <summary>
        /// Number of data sets
        /// </summary>
        public int Count { get { return this.List.Count; } }
        /// <summary>
        /// The last ranking of the previous set
        /// </summary>
        public int SeedRankNumber { get; set; }
        /// <summary>
        /// Current set ranking starting value
        /// </summary>
        public int BeginRankNumber { get { return this.SeedRankNumber + 1; } }
        /// <summary>
        /// Current set ranking end value
        /// </summary>
        public int EndRankNumber { get { return this.SeedRankNumber + this.Count; } }
        /// <summary>
        /// Cache group ID
        /// </summary>
        public long GroupIndex { get; }
        /// <summary>
        /// The set of the previous group
        /// </summary>
        public CustomerRankGroup Prev { get; set; }
        /// <summary>
        /// The set of the next group
        /// </summary>
        public CustomerRankGroup Next { get; set; }

        public CustomerRankGroup(long groupIndex)
        {
            this.GroupIndex = groupIndex;
        }
    }
}
