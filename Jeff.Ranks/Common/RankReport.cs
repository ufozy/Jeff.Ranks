using Jeff.Ranks.Models;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace Jeff.Ranks.Common
{
    public class RankReport
    {
        /// <summary>
        /// Find objects within a specified ranking range
        /// </summary>
        /// <param name="start">start</param>
        /// <param name="end">end</param>
        /// <returns></returns>
        public static List<CustomerRank> RankSection(long start, long end)
        {
            var result = new List<CustomerRank>();
            var list = Cache.GroupRanks.Values
                .Where(
                t =>
                t.BeginRankNumber <= end && t.EndRankNumber >= start
                );      //Find a specific cache region set

            foreach (var item in list)
            //Iterate through the specified cache region set
            {
                var realRankList = item.List.FindAll(t => t.Rank + item.SeedRankNumber >= start && t.Rank + item.SeedRankNumber <= end)
                    .Select(t => t.GetRealRank(item.SeedRankNumber));
                result.AddRange(realRankList);
            }

            return result;
        }

        /// <summary>
        /// Find the specified element and its nearby rankings.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="high"></param>
        /// <param name="low"></param>
        /// <returns></returns>
        public static List<CustomerRank> RankSection(long customerId, long high, long low)
        {
            var result = new List<CustomerRank>();
            if (!Cache.CustomerRank.ContainsKey(customerId))
            {
                return result;
            }

            var customer = Cache.CustomerRank[customerId];

            if (customer.Score < 0)
                return result;

            var group = GroupRanksUtils.GetGroupRanksByScore(customer.Score);

            var rank = group.SeedRankNumber + customer.Rank;
            var start = rank - high;
            var end = rank + low;

            return RankSection(start, end);
        }
    }
}
