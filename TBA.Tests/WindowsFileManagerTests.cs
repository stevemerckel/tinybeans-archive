using System.IO;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests
{
    [TestFixture]
    public sealed class WindowsFileManagerTests : BaseFileManagerTests
    {
        private static readonly IFileManager _windowsFileManager = new WindowsFileSystemManager();

        public WindowsFileManagerTests() : base(_windowsFileManager)
        {
        }

        [Test]
        public void Test_EnsureBackslashAsPathSeparator_Success()
        {
            Assert.AreEqual('\\', Path.DirectorySeparatorChar);
        }
    }
}