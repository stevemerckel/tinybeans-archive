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
                Assert.Multiple(() =>
                {
                    Assert.IsTrue(d.DisplayedOn > new DateTime(1900, 1, 1));

                    if (d.ArchiveType == ArchiveType.Text)
                    {
                        Assert.IsTrue(string.IsNullOrWhiteSpace(d.SourceUrl));
                        Assert.Throws<ArgumentNullException>(() => new Uri(d.SourceUrl));
                    }
                    else
                    {
                        Assert.IsFalse(string.IsNullOrWhiteSpace(d.SourceUrl));
                        Assert.DoesNotThrow(() => new Uri(d.SourceUrl));
                    }
                });
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
                Assert.Multiple(() =>
                {
                    Assert.IsTrue(yme.DisplayedOn > new DateTime(1970, 1, 1));

                    if (yme.ArchiveType == ArchiveType.Text)
                    {
                        Assert.True(string.IsNullOrWhiteSpace(yme.SourceUrl));
                        Assert.Throws<ArgumentNullException>(() => new Uri(yme.SourceUrl));
                    }
                    else
                    {
                        Assert.IsFalse(string.IsNullOrWhiteSpace(yme.SourceUrl));
                        Assert.DoesNotThrow(() => new Uri(yme.SourceUrl));
                    }
                });
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
    }
}