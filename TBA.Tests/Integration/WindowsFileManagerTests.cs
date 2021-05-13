using NUnit.Framework;
using System.IO;
using TBA.Common;

namespace TBA.Tests.Integration
{
    /// <summary>
    /// Tests of the <see cref="IFileManager"/> contract using the Windows file system
    /// </summary>
    [TestFixture]
    public sealed class WindowsFileManagerTests : BaseFileManagerTests
    {
        private static readonly IFileManager _sut = new WindowsFileSystemManager();

        public WindowsFileManagerTests() : base(_sut)
        {
        }

        [Test]
        public void Test_Dummy_Success()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public override void Test_EnsureProperPathSeparatorByHost_Success()
        {
            Assert.AreEqual(Path.DirectorySeparatorChar, _sut.DirectorySeparatorChar);
            DefaultMocks.MockLogger.Info($"The override implementation of '{nameof(Test_EnsureProperPathSeparatorByHost_Success)}' did run!");
        }
    }
}