using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;
using TBA.Common;

namespace TBA.Tests
{
    [TestFixture]
    public class ObjectTests
    {
        private readonly string _jsonSamplesLocation = Path.Combine(TestContext.CurrentContext.TestDirectory, "json-samples");

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

            var dayEntry = JsonConvert.DeserializeObject<ArchivedContent>(json);
            Assert.IsNotNull(dayEntry);
            Assert.Multiple(() =>
            {
                Assert.IsTrue(dayEntry.DisplayedOn > new DateTime(1900, 1, 1));

                // todo: enable section below once URL pass-through is working
                //Assert.IsFalse(string.IsNullOrWhiteSpace(dayEntry.SourceUrl));
                //Assert.DoesNotThrow(() => new Uri(dayEntry.SourceUrl));
            });
        }

        [Test]
        public void Test_Deserialize_YearMonthEntries_Success()
        {
            const string FileName = "year-month-entries.json";
            var jsonLocation = Path.Combine(_jsonSamplesLocation, FileName);
            var json = CommonJsonFileAssertionsAndReturnContent(jsonLocation);

            var yearMonthEntries = JsonConvert.DeserializeObject<List<ArchivedContent>>(json);
            Assert.IsNotNull(yearMonthEntries);
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

            var summaries = JsonConvert.DeserializeObject<List<JournalSummary>>(json);
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

        [Test]
        public void Test_Deserialize_DayEntries_Fail()
        {
            const string FileName = "day-entries.json";
            var jsonLocation = Path.Combine(_jsonSamplesLocation, FileName);
            var json = CommonJsonFileAssertionsAndReturnContent(jsonLocation);
            const int MangleSplitAt = 30;
            var mangled = json.Substring(0, MangleSplitAt);
            mangled += "This_Got_Mangled";
            mangled += json.Substring(MangleSplitAt + 1);

            Assert.Catch(() => JsonConvert.DeserializeObject<ArchivedContent>(mangled));
        }

        [Test]
        public void Test_Deserialize_YearMonthEntries_Fail()
        {
            const string FileName = "year-month-entries.json";
            var jsonLocation = Path.Combine(_jsonSamplesLocation, FileName);
            var json = CommonJsonFileAssertionsAndReturnContent(jsonLocation);
            const int MangleSplitAt = 30;
            var mangled = json.Substring(0, MangleSplitAt);
            mangled += "This_Got_Mangled";
            mangled += json.Substring(MangleSplitAt + 1);

            Assert.Catch(() => JsonConvert.DeserializeObject<List<ArchivedContent>>(mangled));
        }

        [Test]
        public void Test_Deserialize_JournalSummary_Fail()
        {
            const string FileName = "journal-summary.json";
            var jsonLocation = Path.Combine(_jsonSamplesLocation, FileName);
            var json = CommonJsonFileAssertionsAndReturnContent(jsonLocation);
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