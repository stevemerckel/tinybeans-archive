using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests.Integration
{
    /// <summary>
    /// Integration tests for the Tinybeans API
    /// </summary>
    [TestFixture]
    public sealed class TinybeansApiTests : BaseTinybeansApiTests
    {
        private static readonly IFileManager _fileManager;
        private static readonly IRuntimeSettingsProvider _runtimeSettingsProvider;
        private static readonly ITinybeansJsonHelper _jsonHelper;
        private static readonly ITinybeansApiHelper _sut;

        /// <remarks>
        /// Initializes the static objects for later consumption by the default ctor
        /// </remarks>
        static TinybeansApiTests()
        {
            _fileManager = new WindowsFileSystemManager();
            _runtimeSettingsProvider = new RuntimeSettingsProvider(_fileManager);
            _jsonHelper = new TinybeansJsonHelper(DefaultMocks.MockLogger);
            _sut = new TinybeansApiHelper(DefaultMocks.MockLogger, _runtimeSettingsProvider, _jsonHelper, _fileManager);
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        public TinybeansApiTests() : base(_sut)
        {
        }

        [Test]
        public void Test_DummyTest_Success()
        {
            Assert.IsTrue(true);
        }
    }
}