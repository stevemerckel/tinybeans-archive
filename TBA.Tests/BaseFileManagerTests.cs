using System;
using System.Collections.Generic;
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
        protected readonly string RuntimeLocation = TestContext.CurrentContext.TestDirectory;
        private readonly IAppLogger _logger = DefaultMocks.MockLogger;
        private string _workLocation;

        public BaseFileManagerTests(IFileManager implementation)
        {
            _sut = implementation;
        }

        public abstract void Test_EnsureProperPathSeparatorByHost_Success();

        [SetUp]
        public void InitializeTest()
        {
            Assert.IsNotNull(_sut);
            Assert.IsFalse(string.IsNullOrWhiteSpace(RuntimeLocation));
            Assert.IsTrue(_sut.DirectoryExists(RuntimeLocation));

            // critical: generate the subfolder "work" location in file system
            var workFolderName = GenerateTempDirectoryName();
            _workLocation = _sut.PathCombine(RuntimeLocation, workFolderName);
            _sut.CreateDirectory(_workLocation);
            Assert.IsTrue(_sut.DirectoryExists(_workLocation));

            _logger.Info($"Finished {nameof(InitializeTest)}");
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
            var list = new List<string>
            {
                "Line 01",
                "Line Two",
                "Line #3"
            };

            Assert.Greater(list.Count, 1, "More than one line is needed in the list of strings to meet test requirements !!");

            var sb = new StringBuilder();
            list.ForEach(x => sb.AppendLine(x));

            var content = sb.ToString();

            var fileLocation = _sut.PathCombine(_workLocation, GenerateTempFileName("txt"));
            await _sut.FileWriteTextAsync(fileLocation, content);
            Assert.IsTrue(_sut.FileExists(fileLocation));
            Assert.IsTrue(_sut.FileSize(fileLocation) > 0);
            var readIn = _sut.FileReadAllText(fileLocation);
            Assert.AreEqual(content, readIn);
            var newLineCharacterLength = Environment.NewLine.Length;
            var newLineCount = (readIn.Length - readIn.Replace(Environment.NewLine, string.Empty).Length) / newLineCharacterLength;
            Assert.AreEqual(list.Count, newLineCount);
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

            // generate hash off the new temp file
            var hash = _sut.FileHash(fileLocation);
            _logger.Info($"hash: {hash}");

            // validate that only hexadecimal characters are in the hash
            var chars = hash.ToCharArray();
            foreach (var c in chars)
            {
                var isHex = (c >= '0' && c <= '9')
                    || (c >= 'A' && c <= 'F')
                    || (c >= 'a' && c <= 'f');
                
                Assert.IsTrue(isHex, $"character '{c}' was found in hash that should only  hexadecimal hash of '{hash}' !!");
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