using Moq;
using NUnit.Framework;
using System;
using System.Linq;
using TBA.Common;

namespace TBA.Tests.Unit
{
    /// <summary>
    /// Unit tests for the <see cref="ITinybeansApiHelper"/> object
    /// </summary>
    [TestFixture]
    public sealed class TinybeansApiTests : BaseTinybeansApiTests
    {
        private const string DayEntriesFileName = "day-entries.json";
        private const string JournalSummaryFileName = "journal-summary.json";
        private static readonly string _jsonSamplesLocation;
        private static readonly Mock<ITinybeansApiHelper> _mock = GetMock();
        private static readonly IFileManager _fileManager = new WindowsFileSystemManager();
        
        static TinybeansApiTests()
        {
            _jsonSamplesLocation = _fileManager.PathCombine(TestContext.CurrentContext.TestDirectory, "json-samples");
        }

        public TinybeansApiTests() : base(_mock.Object)
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
                    var localPath = _fileManager.PathCombine(path, "source-file.ext");
                    var squarePath = _fileManager.PathCombine(path, "square-image.ext");
                    var rectPath = _fileManager.PathCombine(path, "rectangle-image.ext");
                    return new EntryDownloadInfo(entry.Id, localPath, squarePath, rectPath);
                });
            mockApi
                .Setup(x => x.GetByDate(It.IsAny<DateTime>(), It.IsAny<long>()))
                .Returns<DateTime, long>((targetDate, journalId) =>
                {
                    var dayJsonLocation = _fileManager.PathCombine(_jsonSamplesLocation, DayEntriesFileName);
                    var pool = jsonHelper.ParseArchivedContent(_fileManager.FileReadAllText(dayJsonLocation));
                    if (!pool.Any())
                        throw new Exception("No entries found from parsed JSON file!");

                    var matchesByDate = pool.Where(x => x.DisplayedOn.Date == targetDate.Date).ToList();
                    if (!matchesByDate.Any())
                        throw new Exception($"No matches found in day '{targetDate.ToString("yyyy-MM-dd")}' from mock out of {pool.Count} entries.");

                    return matchesByDate.ToList();
                });
            mockApi
                .Setup(x => x.GetEntriesByYearMonth(It.IsAny<DateTime>(), It.IsAny<long>()))
                .Returns<DateTime, long>((targetDate, journalId) =>
                {
                    var dayJsonLocation = _fileManager.PathCombine(_jsonSamplesLocation, DayEntriesFileName);
                    var pool = jsonHelper.ParseArchivedContent(_fileManager.FileReadAllText(dayJsonLocation));
                    if (!pool.Any())
                        throw new Exception("No entries found from parsed JSON file!");

                    var start = new DateTime(targetDate.Year, targetDate.Month, 1, 0, 0, 0, DateTimeKind.Utc);
                    var end = start.AddMonths(1);
                    var matchesByMonthRange = pool.Where(x => x.DisplayedOn.Date >= start.Date && x.DisplayedOn.Date < end.Date).ToList();
                    if (!matchesByMonthRange.Any())
                        throw new Exception($"No matches found in month of '{start.ToString("MMMM yyyy")}' from mock out of {pool.Count} entries.");

                    return matchesByMonthRange.ToList();
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