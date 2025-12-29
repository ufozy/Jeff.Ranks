using Jeff.Ranks.Models;

namespace Jeff.Ranks.Common
{
    public static class GroupRanksUtils
    {
        /// <summary>
        /// Calculate the specific cache region using the score and return it.
        /// </summary>
        /// <param name="score">score</param>
        /// <returns>The specific cache region</returns>
        public static CustomerRankGroup GetGroupRanksByScore(decimal score)
        {
            if (score <= 0)
                return null;

            var i = (long)Math.Floor(score / Cache.GroupSize);
            return GetGroupRanks(i);
        }

        /// <summary>
        /// Returns a specific region using the cache region ID. If the specified region does not exist, it will be created first.
        /// </summary>
        /// <param name="groupIndex">The cache region ID</param>
        /// <returns>The specific cache region</returns>
        public static CustomerRankGroup GetGroupRanks(long groupIndex)
        {
            if (!Cache.GroupRanks.ContainsKey(groupIndex))
            // The specified region does not exist in the cache.
            {
                var newGroup = new CustomerRankGroup(groupIndex);

                if (Cache.GroupRanks.Count == 0)
                // The cache is empty.
                {
                    Cache.GroupRanks[groupIndex] = newGroup;
                    return newGroup;
                }

                var startIndex = Cache.GroupRanks.Keys.Max();       //Get the cache header ID
                var head = Cache.GroupRanks[startIndex];

                if (head.GroupIndex < newGroup.GroupIndex)
                //Insert from the head of the queue
                {
                    head.Prev = newGroup;
                    newGroup.Next = head;
                    head.SeedRankNumber = head.Prev.SeedRankNumber;
                }
                else
                {
                    while (true)
                    {
                        if (head.Next == null)
                        //Insert from the tail of the queue
                        {
                            head.Next = newGroup;
                            newGroup.Prev = head;
                            newGroup.SeedRankNumber = newGroup.Prev.EndRankNumber;

                            break;
                        }

                        if (head.GroupIndex < newGroup.GroupIndex)
                        //Insert from the middle of the queue
                        {
                            head.Prev.Next = newGroup;
                            newGroup.Prev = head.Prev;
                            head.Prev = newGroup;
                            newGroup.Next = head;
                            newGroup.SeedRankNumber = newGroup.Prev.EndRankNumber;

                            break;
                        }

                        head = head.Next;
                    }
                }

                Cache.GroupRanks[groupIndex] = newGroup;    //New regions added to cache collection
            }
            return Cache.GroupRanks[groupIndex];
        }
    }
}
