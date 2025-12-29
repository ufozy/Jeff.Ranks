using Jeff.Ranks.Models;

namespace Jeff.Ranks.Common
{
    public static class Cache
    {
        /// <summary>
        /// The size of each cache group's range. This can be set according to actual business needs.
        /// </summary>
        public static long GroupSize = 10000;
        public static Dictionary<long, CustomerRank> CustomerRank { get; set; } = new Dictionary<long, CustomerRank>();
        public static SortedDictionary<long, CustomerRankGroup> GroupRanks { get; set; } = new SortedDictionary<long, CustomerRankGroup>(Comparer<long>.Create((x, y) => y.CompareTo(x)));
    }
}
