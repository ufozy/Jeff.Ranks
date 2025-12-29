using Jeff.Ranks.Models;

namespace Jeff.Ranks.Common
{
    public class CustomerRankHandle
    {
        private readonly long customerId;
        private CustomerRank Customer;

        private CustomerRankGroup OldRankGroup = null;
        private CustomerRankGroup NewRankGroup = null;

        public CustomerRankHandle(long customerId)
        {
            this.customerId = customerId;

            if (!Cache.CustomerRank.ContainsKey(customerId))
            {
                Cache.CustomerRank[customerId] = new CustomerRank(customerId);
            }

            Customer = Cache.CustomerRank[customerId];
        }

        /// <summary>
        /// Update score
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public CustomerRank SetScore(decimal score)
        {
            if(score>1000 ||score < -1000)
            {
                throw new BusinessException("001", "The score range is -1000 to 1000.");
            }

            OldRankGroup = GroupRanksUtils.GetGroupRanksByScore(this.Customer.Score);   //Locate the old cache area
            this.Customer.Score += score;
            NewRankGroup = GroupRanksUtils.GetGroupRanksByScore(this.Customer.Score);   //Locate the new cache area

            RefreshRank();

            if (this.Customer.Score > 0)
                return this.Customer.GetRealRank(NewRankGroup.SeedRankNumber);
            else
                return this.Customer;
        }

        /// <summary>
        /// Get the starting position of the affected area
        /// </summary>
        /// <returns></returns>
        private CustomerRankGroup GetHead()
        {
            if (OldRankGroup == null && NewRankGroup == null)
            {
                return null;
            }

            if (OldRankGroup == null)
                return NewRankGroup;
            if (NewRankGroup == null)
                return OldRankGroup;

            return OldRankGroup.GroupIndex > NewRankGroup.GroupIndex ? OldRankGroup : NewRankGroup;
        }

        /// <summary>
        /// Get the end position of the affected area
        /// </summary>
        /// <returns></returns>
        private CustomerRankGroup GetEnd()
        {
            if (OldRankGroup == null || NewRankGroup == null)
            {
                return null;
            }

            return OldRankGroup.GroupIndex > NewRankGroup.GroupIndex ? NewRankGroup : OldRankGroup;
        }

        /// <summary>
        /// Refresh the ranking within the relevant cache area.
        /// </summary>
        private void AdjustSeedRankNumber()
        {
            CustomerRankGroup head = GetHead();
            CustomerRankGroup end = GetEnd();

            if (head.Prev == null)
                head = head.Next;

            while (head != null && (end == null || head.GroupIndex >= end.GroupIndex))
            {
                head.SeedRankNumber = head.Prev.EndRankNumber;
                head = head.Next;
            }
        }

        /// <summary>
        /// Update rankings and adjust cache location
        /// </summary>
        public void RefreshRank()
        {
            if (OldRankGroup == null && NewRankGroup == null)
                //Not participating in ranking
                return;

            else if (OldRankGroup != null && NewRankGroup == null)
            //Removed from rankings
            {
                RemoveCustomerRankFromGroup();
            }

            else if (OldRankGroup == null && NewRankGroup != null)
            //Newcomer Ranking
            {
                EntryCustomerToGroup();
            }
            else if (OldRankGroup != null && NewRankGroup != null)
            //Adjust the position in the cache
            {
                if (OldRankGroup.GroupIndex == NewRankGroup.GroupIndex)
                //Move within the original cache area
                {
                    EntryCustomerToGroup();
                }
                else if (OldRankGroup.GroupIndex < NewRankGroup.GroupIndex)
                //Move to the previous cache area
                {
                    MoveFoward();
                }
                else
                //Move to the later cache area
                {
                    MoveBack();
                }
            }

            AdjustSeedRankNumber();         //Refresh the ranking within the relevant cache area.
        }

        /// <summary>
        /// Insert into a specific cache area
        /// </summary>
        public void EntryCustomerToGroup()
        {
            
            var rankIndex = NewRankGroup.List
                .Where(t => t.Score > Customer.Score || t.Score == Customer.Score && t.CustomerId < Customer.CustomerId)
                .Count();   //Calculate the relative rank within the current cache region.

            if (Customer.Rank != -1)
            //If it has already been removed in the area
            {
                NewRankGroup.List.Remove(Customer);
            }
            Customer.Rank = rankIndex;  //Initialize a relative ranking. This ranking is not real.
            NewRankGroup.List.Insert(rankIndex, Customer);  //Insert elements at specified positions, ensuring the order of elements is not disrupted.

            var remains = NewRankGroup.List
                    .Where(t => t.Score < Customer.Score 
                    || t.Score == Customer.Score 
                    && t.CustomerId >= Customer.CustomerId);    //Find out elements whose relative rankings need to be updated.

            foreach (var item in remains)
            //The remaining elements include the current element whose rank has been shifted one position to the right.
            {
                item.Rank = ++rankIndex;
            }
        }

        /// <summary>
        /// Remove the current object from the current group. 
        /// Also, move the rank of any element immediately following the current object forward by one position.
        /// </summary>
        public void RemoveCustomerRankFromGroup()
        {
            OldRankGroup.List.Remove(Customer);
            OldRankGroup.List.FindAll(t => t.Rank > Customer.Rank).ForEach(t => t.Rank--);
            Customer.Rank = -1; //Reset Rank
        }

        /// <summary>
        /// Move to the later cache area
        /// </summary>
        public void MoveBack()
        {
            RemoveCustomerRankFromGroup();

            var head = OldRankGroup.Next;
            while (head != null)
            {
                if (head.GroupIndex == NewRankGroup.GroupIndex)
                {
                    EntryCustomerToGroup();
                    break;
                }

                head = head.Next;
            }
        }

        /// <summary>
        /// Move to the previous cache area
        /// </summary>
        public void MoveFoward()
        {
            RemoveCustomerRankFromGroup();

            var head = OldRankGroup;
            while (head != null)
            {
                if (head.GroupIndex == NewRankGroup.GroupIndex)
                {
                    EntryCustomerToGroup();
                    break;
                }

                head = head.Prev;
            }
        }
    }
}
