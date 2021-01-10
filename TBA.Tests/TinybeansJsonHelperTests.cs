using System;
using System.IO;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests
{
    [TestFixture]
    public class TinybeansJsonHelperTests : TestBase
    {
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

        [Test]
        public void Test_Deserialize_DayEntry_Success()
        {
            const string FileName = "day-entries.json";
            var jsonLocation = Path.Combine(_jsonSamplesLocation, FileName);
            var json = CommonJsonFileAssertionsAndReturnContent(jsonLocation);

            var dayEntries = _sut.ParseArchivedContent(json);
            Assert.IsNotNull(dayEntries);
            Assert.IsTrue(dayEntries.Count > 0);
            dayEntries.ForEach(d =>
            {
                Assert.Multiple(() =>
                {
                    Assert.IsTrue(d.DisplayedOn > new DateTime(1900, 1, 1));

                    // todo: enable section below once URL pass-through is working
                    //Assert.IsFalse(string.IsNullOrWhiteSpace(dayEntry.SourceUrl));
                    //Assert.DoesNotThrow(() => new Uri(dayEntry.SourceUrl));
                });
            });
        }

        [Test]
        public void Test_Deserialize_YearMonthEntries_Success()
        {
            const string FileName = "year-month-entries.json";
            var jsonLocation = Path.Combine(_jsonSamplesLocation, FileName);
            var json = CommonJsonFileAssertionsAndReturnContent(jsonLocation);

            var yearMonthEntries = _sut.ParseArchivedContent(json);
            Assert.IsNotNull(yearMonthEntries);
            Assert.IsTrue(yearMonthEntries.Count > 0);
            yearMonthEntries.ForEach(yme =>
            {
                Assert.Multiple(() =>
                {
                    Assert.IsTrue(yme.DisplayedOn > new DateTime(1900, 1, 1));

                    // todo: enable section below once URL pass-through is working
                    //Assert.IsFalse(string.IsNullOrWhiteSpace(dayEntry.SourceUrl));
                    //Assert.DoesNotThrow(() => new Uri(dayEntry.SourceUrl));
                });
            });
        }

        [Test]
        public void Test_Deserialize_JournalSummary_Success()
        {
            const string FileName = "journal-summary.json";
            var jsonLocation = Path.Combine(_jsonSamplesLocation, FileName);
            var json = CommonJsonFileAssertionsAndReturnContent(jsonLocation);

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
        /// Wraps common pre-checks on the json file
        /// </summary>
        /// <param name="fileLocation">The JSON file location to be inspected</param>
        /// <returns>The JSON content from the location</returns>
        private string CommonJsonFileAssertionsAndReturnContent(string fileLocation)
        {
            Assert.IsTrue(File.Exists(fileLocation), $"JSON file not found: {fileLocation}");
            var fi = new FileInfo(fileLocation);
            Assert.IsNotNull(fi);
            Assert.AreNotEqual(0, fi.Length);
            var content = File.ReadAllText(fileLocation);
            Assert.IsTrue(content.Length > 100);
            Assert.IsFalse(string.IsNullOrWhiteSpace(content));
            return content;
        }
    }
}