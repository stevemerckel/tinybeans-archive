using System;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests.Integration
{
    [TestFixture]
    public class TinybeansJsonHelperTests : TestBase
    {
        private const string DayEntriesFileName = "day-entries.json";
        private const string JournalSummaryFileName = "journal-summary.json";
        private readonly ITinybeansJsonHelper _sut;
        private readonly string _jsonSamplesLocation;
        const string JsonStructureValidButNotAligned = @"{ ""one"": 1, ""two"": 2, ""three"": 3 }";
        const string JsonInvalid = @"This Is Not Json";
        private readonly IFileManager _fileManager = new WindowsFileSystemManager();

        public TinybeansJsonHelperTests()
        {
            _sut = new TinybeansJsonHelper(DefaultMocks.MockLogger);
            _jsonSamplesLocation = _fileManager.PathCombine(TestExecutionDirectory, "json-samples");
        }

        [OneTimeSetUp]
        public void EnsureJsonSamplesDirectoryExists()
        {
            Assert.IsTrue(_fileManager.DirectoryExists(_jsonSamplesLocation));
        }

        [TestCase(DayEntriesFileName)]
        [TestCase(JournalSummaryFileName)]
        public void Test_JsonSamplesHaveContent_Success(string fileName)
        {
            var location = _fileManager.PathCombine(_jsonSamplesLocation, fileName);
            Assert.IsTrue(_fileManager.FileExists(location));
            Assert.IsTrue(_fileManager.FileSize(location) > 0);
            var content = _fileManager.FileReadAllText(location);
            Assert.IsTrue(content.Length > 100);
            Assert.IsFalse(string.IsNullOrWhiteSpace(content));
        }

        [Test]
        public void Test_Deserialize_DayEntry_Success()
        {
            var jsonLocation = _fileManager.PathCombine(_jsonSamplesLocation, DayEntriesFileName);
            var json = _fileManager.FileReadAllText(jsonLocation);

            var dayEntries = _sut.ParseArchivedContent(json);
            Assert.IsNotNull(dayEntries);
            Assert.IsTrue(dayEntries.Count > 0);
            dayEntries.ForEach(d =>
            {
                ValidateArchiveEntryDataAgainstRules(d);
            });
        }

        [Test]
        public void Test_Deserialize_JournalSummary_Success()
        {
            var jsonLocation = _fileManager.PathCombine(_jsonSamplesLocation, JournalSummaryFileName);
            var json = _fileManager.FileReadAllText(jsonLocation);

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

        [TestCase(JsonInvalid)]
        [TestCase(JsonStructureValidButNotAligned)]
        public void Test_Deserialize_DayEntries_BogusData_Fail(string json)
        {
            Assert.Catch(() => _sut.ParseArchivedContent(json));
        }

        [TestCase(JsonInvalid)]
        [TestCase(JsonStructureValidButNotAligned)]
        public void Test_Deserialize_YearMonthEntries_BogusData_Fail(string json)
        {
            Assert.Catch(() => _sut.ParseArchivedContent(json));
        }

        [TestCase(JsonInvalid)]
        [TestCase(JsonStructureValidButNotAligned)]
        public void Test_Deserialize_JournalSummary_BogusData_Fail(string json)
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