using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Utilities
{
    public class WeightedRange : Dictionary<int,int>
    {
        public int GetTotalWeight()
        {
            int total = 0;
            foreach (var value in Values)
            {
                total += value;
            }

            return total;
        }

        public int GetWeightedRandom()
        {
            var weight = GetTotalWeight();
            var random = Random.Range(0, weight);
            var totalWeight = 0;

            foreach (KeyValuePair<int, int> entry in this)
            {
                totalWeight += entry.Value;
                if (totalWeight > random)
                {
                    return entry.Key;
                }
            }

            return Keys.First();
        }
        
    }
}