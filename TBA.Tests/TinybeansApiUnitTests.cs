using Moq;
using NUnit.Framework;
using System;
using TBA.Common;

namespace TBA.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="ITinybeansApiHelper"/> object
    /// </summary>
    [TestFixture]
    public sealed class TinybeansApiUnitTests : TinybeansApiBaseTests
    {
        private const string DayEntriesFileName = "day-entries.json";
        private const string JournalSummaryFileName = "journal-summary.json";
        private static readonly string _jsonSamplesLocation = System.IO.Path.Combine(TestContext.CurrentContext.TestDirectory, "json-samples");
        private static readonly Mock<ITinybeansApiHelper> _mock = GetMock();
        private static readonly IFileManager _fileManager = new WindowsFileSystemManager();
        
        public TinybeansApiUnitTests() : base(_mock.Object)
        {

        }

        [SetUp]
        public void Test_ValidateJsonFilesExist_Success()
        {
            Assert.IsTrue(_fileManager.DirectoryExists(_jsonSamplesLocation));
            Assert.IsTrue(_fileManager.FileExists(_fileManager.PathCombine(_jsonSamplesLocation, DayEntriesFileName)));
            Assert.IsTrue(_fileManager.FileExists(_fileManager.PathCombine(_jsonSamplesLocation, JournalSummaryFileName)));
        }

        private static Mock<ITinybeansApiHelper> GetMock()
        {
            var mockApi = new Mock<ITinybeansApiHelper>();

            var jsonHelper = new TinybeansJsonHelper(DefaultMocks.MockLogger);

            mockApi
                .Setup(x => x.Download(It.IsAny<ITinybeansEntry>(), It.IsAny<string>()))
                .Returns<ITinybeansEntry, string>((entry, path) =>
                {
                    var localPath = System.IO.Path.Combine(path, "source-file.ext");
                    var squarePath = System.IO.Path.Combine(path, "square-image.ext");
                    var rectPath = System.IO.Path.Combine(path, "rectangle-image.ext");
                    return new EntryDownloadInfo(entry.Id, localPath, squarePath, rectPath);
                });
            mockApi
                .Setup(x => x.GetByDate(It.IsAny<DateTime>(), It.IsAny<long>()))
                .Returns<DateTime, long>((targetDate, journalId) =>
                {
                    var dayJsonLocation = _fileManager.PathCombine(_jsonSamplesLocation, DayEntriesFileName);
                    return jsonHelper.ParseArchivedContent(_fileManager.FileReadAllText(dayJsonLocation));
                });
            mockApi
                .Setup(x => x.GetEntriesByYearMonth(It.IsAny<DateTime>(), It.IsAny<long>()))
                .Returns<DateTime, long>((targetDate, journalId) =>
                {
                    // NOTE: This is a little cheating by calling the "single day" implementation, but the JSON structure they use for "single day" and "year-month" is the same.
                    //       Worst case is that this mocked test will succeed while the integration test will fail.
                    //       And to be fair, that is possible given that Tinybeans has not (yet!) published their API.
                    //       So we'll do the best we can within reason  :)
                    return mockApi.Object.GetByDate(targetDate, journalId);
                });
            mockApi
                .Setup(x => x.GetJournalSummaries())
                .Returns(() =>
                {
                    var summaryJsonLocation = _fileManager.PathCombine(_jsonSamplesLocation, JournalSummaryFileName);
                    return jsonHelper.ParseJournalSummaries(_fileManager.FileReadAllText(summaryJsonLocation));
                });

            return mockApi;
        }
    }
}