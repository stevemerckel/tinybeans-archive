using System.IO;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests.Integration
{
    /// <summary>
    /// Tests of the <see cref="IFileManager"/> contract using the Windows file system
    /// </summary>
    [TestFixture]
    public sealed class WindowsFileManagerTests : BaseFileManagerTests
    {
        private static readonly IFileManager _windowsFileManager = new WindowsFileSystemManager();

        public WindowsFileManagerTests() : base(_windowsFileManager)
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
            Assert.AreEqual('\\', Path.DirectorySeparatorChar);
            DefaultMocks.MockLogger.Info($"The override implementation of '{nameof(Test_EnsureProperPathSeparatorByHost_Success)}' did run!");
        }
    }
}