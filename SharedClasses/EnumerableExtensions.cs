using System.Collections.Generic;
using System.Linq;

namespace SharedClasses
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Checks if two lists contain the same items in any order.
        /// </summary>
        /// <typeparam name="T">Type of element in the collection.</typeparam>
        /// <param name="first">The first list of unordered items.</param>
        /// <param name="second">The second list of unordered items.</param>
        /// <returns>Boolean result of if lists contain same elements.</returns>
        public static bool HasSameElementsAs<T>(this IEnumerable<T> first, IEnumerable<T> second)
        {
            Dictionary<T, int> firstMap = first.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
            Dictionary<T, int> secondMap = second.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());

            return firstMap.Keys.All(x => secondMap.Keys.Contains(x) && firstMap[x] == secondMap[x]) &&
                   secondMap.Keys.All(x => firstMap.Keys.Contains(x) && secondMap[x] == firstMap[x]);
        }
    }
}