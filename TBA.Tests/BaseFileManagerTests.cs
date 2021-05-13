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

        public BaseFileManagerTests(IFileManager implementation)
        {
            _sut = implementation;
        }

        public abstract void Test_EnsureProperPathSeparatorByHost_Success();

        [SetUp]
        public void TestInitialize()
        {
            Assert.IsNotNull(_sut);
            Assert.IsFalse(string.IsNullOrWhiteSpace(RuntimeLocation));
            Assert.IsTrue(_sut.DirectoryExists(RuntimeLocation));
            DefaultMocks.MockLogger.Info($"Finished {nameof(TestInitialize)}");
        }
    }
}