using Jeff.Ranks.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Jeff.Ranks.Common
{
    public class DB
    {
        private readonly IMemoryCache _cache;
        private readonly string cacheKey = "Customer";
        public DB(IMemoryCache cache)
        {
            _cache = cache;
            Customer = new ConcurrentDictionary<long, decimal>() { };
            Ranks = new List<CustomerRank>();
        }

        public ConcurrentDictionary<long, decimal> Customer { get; set; }
        public List<CustomerRank> Ranks { get; set; }

        
    }

    public class aa
    {
        int GroupSize = 10000;
        List<CustomerRank> Ranks { get; set; }
        Dictionary<decimal, List<CustomerRank>> GroupRanks { get; set; } = new Dictionary<decimal, List<CustomerRank>>();


        public void Save(CustomerRank customer,decimal score)
        {
            var newGroupId = Math.Floor(customer.Score / GroupSize);

            if(newGroupId == customer.GroupIndex)
            {
                var ranks = GroupRanks[newGroupId]??new List<CustomerRank>();
                long oldRank = customer.Rank;
                long newRank = ranks.OrderByDescending(t=>t.Rank).First(t=>t.Score > customer.Score)!.Rank;
                if(oldRank > newRank)
                {
                    ranks.FindAll(t => t.Rank < oldRank && t.Rank >= newRank).ForEach(t => t.Rank++);
                }
                else
                {
                    ranks.FindAll(t => t.Rank > oldRank && t.Rank <= newRank).ForEach(t => t.Rank--);
                }

                customer.Rank = newRank;
            }
        }
    }
}
