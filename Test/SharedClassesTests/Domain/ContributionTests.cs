using NUnit.Framework;
using SharedClasses.Domain;

namespace SharedClassesTests.Domain
{
    [TestFixture]
    public class ContributionTests
    {
        [Test]
        public void CompleteContributionTest()
        {
            const string Message = "Hello";

            var finalContribution = new TextContribution(1, new TextContribution(1, Message, 1));
            Assert.AreEqual(finalContribution.Message, Message);
        }

        [Test]
        public void ContributionEqualsTest()
        {
            const string Message = "Hello";

            var contribution = new TextContribution(1, new TextContribution(1, Message, 1));
            var contribution2 = new TextContribution(1, new TextContribution(1, Message, 1));

            Assert.AreEqual(contribution, contribution2);
            Assert.IsTrue(contribution.Equals(contribution2 as object));
        }

        [Test]
        public void ContributionHashCodeTest()
        {
            const string Message = "Hello";

            var contribution = new TextContribution(1, new TextContribution(1, Message, 1));
            var contribution2 = new TextContribution(1, new TextContribution(1, Message, 1));

            Assert.AreEqual(contribution.GetHashCode(), contribution2.GetHashCode());
        }

        [Test]
        public void ContributionReferenceEqualsTest()
        {
            const string Message = "Hello";

            var contribution = new TextContribution(1, new TextContribution(1, Message, 1));

            TextContribution contribution2 = contribution;

            Assert.IsTrue(contribution.Equals(contribution2));
            Assert.IsTrue(contribution.Equals(contribution2 as object));
            Assert.IsFalse(contribution.Equals(null));

            object contributionObject = contribution;

            Assert.IsFalse(contributionObject.Equals(2));
            Assert.IsFalse(contributionObject.Equals(null));
        }

        [Test]
        public void GetContributorUserIdTest()
        {
            const string Message = "Hello";
            const int ContributorUserId = 2;

            var contribution = new TextContribution(ContributorUserId, Message, 1);

            Assert.AreEqual(contribution.ContributorUserId, ContributorUserId);
        }

        [Test]
        public void GetDateTimeTest()
        {
            const string Message = "Hello";

            var finalContribution = new TextContribution(1, new TextContribution(1, Message, 1));
            Assert.IsNotNull(finalContribution.ContributionTimeStamp);
        }

        [Test]
        public void IncompleteContributionTest()
        {
            const string Message = "Hello";
            var contribution = new TextContribution(1, Message, 1);
            Assert.AreEqual(contribution.Message, Message);
        }
    }
}