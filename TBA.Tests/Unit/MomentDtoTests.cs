using System;
using NUnit.Framework;
using TBA.Api;

namespace TBA.Tests.Unit
{
    [TestFixture]
    public sealed class MomentDtoTests : TestBase
    {
        [Test]
        public void Test_ValuesAssignedAreAlignedAndAlwaysTrimmed_Success()
        {
            const long FakeMomentId = 12345;
            const long FakeJournalId = 67890;
            const string GarbagePaddedValue = "     JUNK JUNK JUNK      ";
            var now = DateTime.Now;
            IMomentDto moment = new MomentDto(FakeMomentId, FakeJournalId, GarbagePaddedValue, GarbagePaddedValue, now, GarbagePaddedValue);
            var expectedValue = GarbagePaddedValue.Trim();
            Assert.AreEqual(expectedValue, moment.Caption, $"{nameof(moment.Caption)} was not trimmed!");
            Assert.AreEqual(expectedValue, moment.Type, $"{nameof(moment.Type)} was not trimmed!");
            Assert.AreEqual(expectedValue, moment.Url, $"{nameof(moment.Url)} was not trimmed!");
            Assert.AreEqual(FakeMomentId, moment.Id);
            Assert.AreEqual(FakeJournalId, moment.JournalId);
            Assert.AreEqual(now.Date, moment.LocalDate);
            Assert.AreNotEqual(now, moment.LocalDate);
        }

        [Test]
        public void Test_NullValuesAssignedAreAlwaysStringEmpty_Success()
        {
            IMomentDto moment = new MomentDto(1, 1, null, null, DateTime.Now, null);
            Assert.AreEqual(string.Empty, moment.Caption, $"{nameof(moment.Caption)} was not string.Empty!");
            Assert.AreEqual(string.Empty, moment.Type, $"{nameof(moment.Type)} was not string.Empty!");
            Assert.AreEqual(string.Empty, moment.Url, $"{nameof(moment.Url)} was not string.Empty!");
        }
    }
}