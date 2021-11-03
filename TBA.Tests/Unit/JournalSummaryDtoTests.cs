using System.Collections.Generic;
using NUnit.Framework;
using TBA.Api;

namespace TBA.Tests.Unit
{
    [TestFixture]
    public sealed class JournalSummaryDtoTests : TestBase
    {
        private readonly List<string> _childrenNames = new() { "John Smith", "Jane Smith" };
        private readonly List<string> _parentNames = new() { "Adam", "Eve" };
        private const long FakeJournalId = 123456;

        [Test]
        public void Test_ValuesAssignedAreAlignedAndAlwaysTrimmed_Success()
        {
            const string GarbagePaddedValue = "     JUNK JUNK JUNK      ";
            IJournalSummaryDto summary = new JournalSummaryDto(GarbagePaddedValue, _parentNames, _childrenNames, FakeJournalId);
            var expectedValue = GarbagePaddedValue.Trim();
            Assert.AreEqual(expectedValue, summary.FamilyName, $"{nameof(summary.FamilyName)} was not trimmed!");
            Assert.AreEqual(_parentNames, summary.ParentNames);
            Assert.AreEqual(_childrenNames, summary.ChildrenNames);
            Assert.AreEqual(FakeJournalId, summary.JournalId);
        }

        [Test]
        public void Test_NullValuesAssignedAreAlwaysStringEmpty_Success()
        {
            IJournalSummaryDto summary = new JournalSummaryDto(null, _parentNames, _childrenNames, FakeJournalId);
            Assert.AreEqual(string.Empty, summary.FamilyName, $"{nameof(summary.FamilyName)} was not string.Empty!");
        }
    }
}