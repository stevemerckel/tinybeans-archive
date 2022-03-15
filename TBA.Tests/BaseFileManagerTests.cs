using System;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests
{
    /// <summary>
    /// Base class for holding all common tests/logic for testing file system actions
    /// </summary>
    public abstract class BaseFileManagerTests : TestBase
    {
        private readonly IFileManager _sut;
        protected string RuntimeLocation = TestContext.CurrentContext.TestDirectory;
        private readonly IAppLogger _logger = DefaultMocks.MockLogger;
        private readonly string _workLocation;

        public BaseFileManagerTests(IFileManager implementation)
        {
            _sut = implementation;

            // declare desired subfolder for work tied to the current test
            var workFolderName = GenerateTempDirectoryName();
            _workLocation = _sut.PathCombine(RuntimeLocation, workFolderName);
        }

        public abstract void Test_EnsureProperPathSeparatorByHost_Success();

        [SetUp]
        public void InitializeTest()
        {
            Assert.IsNotNull(_sut);
            Assert.IsFalse(string.IsNullOrWhiteSpace(RuntimeLocation));
            Assert.IsTrue(_sut.DirectoryExists(RuntimeLocation));
            _logger.Info($"Finished {nameof(InitializeTest)}");

            // critical: generate the work location in file system
            _sut.CreateDirectory(_workLocation);
            Assert.IsTrue(_sut.DirectoryExists(_workLocation));
        }

        [TearDown]
        public void TeardownTest()
        {
            if (string.IsNullOrWhiteSpace(_workLocation))
            {
                _logger.Critical($"The '{nameof(_workLocation)}' variable was null/empty !!");
                return;
            }

            if (!_sut.DirectoryExists(_workLocation))
            {
                _logger.Critical($"The '{nameof(_workLocation)}' variable path could not be found !!  looking for '{_workLocation}'");
                return;
            }

            _logger.Info($"Deleting temp folder + contents:  {_workLocation}");
            _sut.DeleteDirectoryAndContents(_workLocation);
        }

        [Test]
        public async Task Test_WriteText_Basic_Success()
        {
            const string Content = "The quick brown fox jumped over the lazy dogs back";

            var fileLocation = _sut.PathCombine(_workLocation, GenerateTempFileName("txt"));
            await _sut.FileWriteTextAsync(fileLocation, Content);
            Assert.IsTrue(_sut.FileExists(fileLocation));
            Assert.IsTrue(_sut.FileSize(fileLocation) > 0);
            var readIn = _sut.FileReadAllText(fileLocation);
            Assert.AreEqual(Content, readIn);
        }

        [Test]
        public async Task Test_WriteText_Multiline_Success()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Line 01");
            sb.AppendLine("Line Two");
            sb.AppendLine("Line #3");

            var content = sb.ToString();

            var fileLocation = _sut.PathCombine(_workLocation, GenerateTempFileName("txt"));
            await _sut.FileWriteTextAsync(fileLocation, content);
            Assert.IsTrue(_sut.FileExists(fileLocation));
            Assert.IsTrue(_sut.FileSize(fileLocation) > 0);
            var readIn = _sut.FileReadAllText(fileLocation);
            Assert.AreEqual(content, readIn);
        }

        [Test]
        public async Task Test_WriteText_ComplexWithEmoji_Success()
        {
            const string Content = "This emoji should be an upside down smiley: 🙃";

            var fileLocation = _sut.PathCombine(_workLocation, GenerateTempFileName("txt"));
            await _sut.FileWriteTextAsync(fileLocation, Content, Encoding.Unicode);
            Assert.IsTrue(_sut.FileExists(fileLocation));
            Assert.IsTrue(_sut.FileSize(fileLocation) > 0);
            var readIn = _sut.FileReadAllText(fileLocation);
            Assert.AreEqual(Content, readIn);
        }

        [Test]
        public async Task Test_MakeFileHash_EnsureOnlyHexadecimalCharacters_Success()
        {
            // write a txt file with random content
            var content = GenerateTempDirectoryName();
            var fileLocation = _sut.PathCombine(_workLocation, GenerateTempFileName("txt"));
            await _sut.FileWriteTextAsync(fileLocation, content);
            Assert.IsTrue(_sut.FileExists(fileLocation));
            Assert.IsTrue(_sut.FileSize(fileLocation) > 0);

            // generate hash
            var hash = _sut.FileHash(fileLocation);
            _logger.Info($"hash: {hash}");

            // validate that only hexadecimal characters are in the hash
            var chars = hash.ToCharArray();
            foreach (var c in chars)
            {
                var isHex = (c >= '0' && c <= '9')
                    || (c >= 'A' && c <= 'F')
                    || (c >= 'a' && c <= 'f');
                
                Assert.IsTrue(isHex, $"character '{c}' was found in hexadecimal hash of '{hash}' !!");
            }
        }

        private static string GenerateTempFileName(string fileExtension)
        {
            var targetExtension = string.IsNullOrWhiteSpace(fileExtension)
                ? string.Empty
                : fileExtension.Trim();

            return $"{Guid.NewGuid().ToString("N").Substring(0, 10)}.{targetExtension}";
        }

        private static string GenerateTempDirectoryName()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 10);
        }
    }
}