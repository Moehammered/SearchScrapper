using System;
using System.Collections.Generic;
using System.Linq;

namespace SearchScraping.Utility
{
    public class PermutationRandomiser<T>
    {
        private readonly Random Randomiser;
        private readonly IList<T> Original;
        private IList<T> PermutationsLeft { get; set; }

        public PermutationRandomiser(in IEnumerable<T> items)
        {
            Original = items.ToList();
            Randomiser = new Random();
            PermutationsLeft = Original;
        }

        public T Next()
        {
            if (Original.Any())
            {
                if (!PermutationsLeft.Any())
                    PermutationsLeft = Original;

                var ind = Randomiser.Next(PermutationsLeft.Count - 1);
                var item = PermutationsLeft[ind];
                PermutationsLeft.RemoveAt(ind);
                return item;
            }
            else
                return default(T);
        }
    }
}
