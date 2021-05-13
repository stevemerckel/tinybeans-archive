using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests.Integration
{
    /// <summary>
    /// Integration tests for Runtime Settings + Provider
    /// </summary>
    [TestFixture]
    public class RuntimeSettingsTests : BaseRuntimeSettingsTests
    {
        private static readonly IRuntimeSettingsProvider _provider = new RuntimeSettingsProvider(new WindowsFileSystemManager());
        private static readonly IFileManager _fileManager = new WindowsFileSystemManager();

        public RuntimeSettingsTests() : base (_provider, false)
        {
        }

        [SetUp]
        public void TestSetup()
        {
            // ensure runtime settings file is found
            const string RuntimeSettingsFileName = "runtime.settings";
            var location = _fileManager.PathCombine(TestExecutionDirectory, RuntimeSettingsFileName);
            Assert.IsTrue(_fileManager.FileExists(location), $"Could not find '{RuntimeSettingsFileName}' here: {location}");
        }
    }
}