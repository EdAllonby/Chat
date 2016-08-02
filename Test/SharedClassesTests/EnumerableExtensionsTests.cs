using System.Collections.Generic;
using NUnit.Framework;
using SharedClasses;

namespace SharedClassesTests
{
    [TestFixture]
    public class EnumerableExtensionsTests
    {
        [Test]
        public void DoesNotHasSameElementsAsTest()
        {
            IEnumerable<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            IEnumerable<int> permutedNumbers = new List<int> { 2, 2, 4, 5, 3 };
            Assert.IsFalse(numbers.HasSameElementsAs(permutedNumbers));
        }

        [Test]
        public void HasSameElementsAsTest()
        {
            IEnumerable<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            IEnumerable<int> permutedNumbers = new List<int> { 2, 1, 4, 5, 3 };
            Assert.IsTrue(numbers.HasSameElementsAs(permutedNumbers));
        }

        [Test]
        public void MissingNumbersInPermutedListTest()
        {
            IEnumerable<int> numbers = new List<int> { 1, 2, 3, 4, 5 };
            IEnumerable<int> permutedNumbers = new List<int> { 4, 5, 3 };
            Assert.IsFalse(numbers.HasSameElementsAs(permutedNumbers));
        }
    }
}