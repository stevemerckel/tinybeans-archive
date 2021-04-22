using System;
using System.IO;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests
{
    [TestFixture]
    public class TinybeansJsonHelperTests : TestBase
    {
        private const string DayEntriesFileName = "day-entries.json";
        private const string YearMonthEntriesFileName = "year-month-entries.json";
        private const string JournalSummaryFileName = "journal-summary.json";
        private readonly ITinybeansJsonHelper _sut;
        private readonly string _jsonSamplesLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, "json-samples");
        const string ValidButNotAlignedJson = @"{ ""one"": 1, ""two"": 2, ""three"":3 }";
        const string InvalidJson = @"This Is Not Json";

        public TinybeansJsonHelperTests()
        {
            _sut = new TinybeansJsonHelper(Logger);
        }

        [OneTimeSetUp]
        public void EnsureJsonSamplesDirectoryExists()
        {
            Assert.IsTrue(Directory.Exists(_jsonSamplesLocation));
        }

        [TestCase(DayEntriesFileName)]
        [TestCase(YearMonthEntriesFileName)]
        [TestCase(JournalSummaryFileName)]
        public void Test_JsonSamplesHaveContent_Success(string fileName)
        {
            var location = Path.Combine(_jsonSamplesLocation, fileName);
            Assert.IsTrue(File.Exists(location));
            var fi = new FileInfo(location);
            Assert.IsNotNull(fi);
            Assert.AreNotEqual(0, fi.Length);
            var content = File.ReadAllText(location);
            Assert.IsTrue(content.Length > 100);
            Assert.IsFalse(string.IsNullOrWhiteSpace(content));
        }

        [Test]
        public void Test_Deserialize_DayEntry_Success()
        {
            var jsonLocation = Path.Combine(_jsonSamplesLocation, DayEntriesFileName);
            var json = File.ReadAllText(jsonLocation);

            var dayEntries = _sut.ParseArchivedContent(json);
            Assert.IsNotNull(dayEntries);
            Assert.IsTrue(dayEntries.Count > 0);
            dayEntries.ForEach(d =>
            {
                ValidateArchiveEntryDataAgainstRules(d);
            });
        }

        [Test]
        public void Test_Deserialize_YearMonthEntries_Success()
        {
            var jsonLocation = Path.Combine(_jsonSamplesLocation, YearMonthEntriesFileName);
            var json = File.ReadAllText(jsonLocation);

            var yearMonthEntries = _sut.ParseArchivedContent(json);
            Assert.IsNotNull(yearMonthEntries);
            Assert.IsTrue(yearMonthEntries.Count > 0);
            yearMonthEntries.ForEach(yme =>
            {
                ValidateArchiveEntryDataAgainstRules(yme);
            });
        }

        [Test]
        public void Test_Deserialize_JournalSummary_Success()
        {
            var jsonLocation = Path.Combine(_jsonSamplesLocation, JournalSummaryFileName);
            var json = File.ReadAllText(jsonLocation);

            var summaries = _sut.ParseJournalSummaries(json);
            summaries.ForEach(s =>
            {
                Assert.Multiple(() =>
                {
                    Assert.IsNotNull(s);
                    Assert.IsTrue(s.CreatedOnUtc >= new DateTime(1900, 1, 1));
                    Assert.IsFalse(string.IsNullOrWhiteSpace(s.Title));
                });
            });
        }

        [TestCase(InvalidJson)]
        [TestCase(ValidButNotAlignedJson)]
        public void Test_Deserialize_DayEntries_Fail(string json)
        {
            Assert.Catch(() => _sut.ParseArchivedContent(json));
        }

        [TestCase(InvalidJson)]
        [TestCase(ValidButNotAlignedJson)]
        public void Test_Deserialize_YearMonthEntries_Fail(string json)
        {
            Assert.Catch(() => _sut.ParseArchivedContent(json));
        }

        [TestCase(InvalidJson)]
        [TestCase(ValidButNotAlignedJson)]
        public void Test_Deserialize_JournalSummary_Fail(string json)
        {
            Assert.Catch(() => _sut.ParseJournalSummaries(json));
        }

        /// <summary>
        /// Conducts a primitive set of inspections against the properties for the archive entry
        /// </summary>
        /// <param name="entry">The archive entry to inspect</param>
        private void ValidateArchiveEntryDataAgainstRules(ITinybeansEntry entry)
        {
            Assert.Multiple(() =>
            {
                // global checks
                Assert.IsTrue(entry.DisplayedOn > new DateTime(1970, 1, 1));
                Assert.IsFalse(string.IsNullOrWhiteSpace(entry.JournalId));
                Assert.IsTrue(ulong.TryParse(entry.JournalId, out ulong temp));

                // per-archive-type checks
                if (entry.ArchiveType == ArchiveType.Text)
                {
                    Assert.True(string.IsNullOrWhiteSpace(entry.SourceUrl));
                    Assert.Throws<ArgumentNullException>(() => new Uri(entry.SourceUrl));
                    Assert.IsFalse(string.IsNullOrWhiteSpace(entry.Caption));
                    return;
                }

                if (entry.ArchiveType == ArchiveType.Image || entry.ArchiveType == ArchiveType.Video)
                {
                    Assert.IsFalse(string.IsNullOrWhiteSpace(entry.SourceUrl));
                    Assert.DoesNotThrow(() => new Uri(entry.SourceUrl));
                    return;
                }

                throw new NotSupportedException($"Incomplete validation checks for archive type '{Enum.GetName(typeof(ArchiveType), entry.ArchiveType)}' !!");
            });
        }
    }
}