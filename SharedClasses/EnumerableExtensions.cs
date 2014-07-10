using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace SharedClasses
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Checks if two collections contain the same elements in any permutation.
        /// </summary>
        /// <typeparam name="T">Type of collections.</typeparam>
        /// <param name="first">The first collections of randomly ordered elements.</param>
        /// <param name="second">The second list of randomly ordered elements.</param>
        /// <returns>Boolean result of if collections contain the same elements.</returns>
        public static bool HasSameElementsAs<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            Contract.Requires(first != null);
            Contract.Requires(second != null);

            Dictionary<T, int> firstMap = first.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            Dictionary<T, int> secondMap = second.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

            return
                firstMap.Keys.All(x => secondMap.Keys.Contains(x) && firstMap[x] == secondMap[x]) &&
                secondMap.Keys.All(x => firstMap.Keys.Contains(x) && secondMap[x] == firstMap[x]);
        }
    }
}