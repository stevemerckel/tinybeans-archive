using System.IO;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests
{
    /// <summary>
    /// Integration tests for Runtime Settings + Provider
    /// </summary>
    [TestFixture]
    public class RuntimeSettingsIntegrationTests : RuntimeSettingsBaseTests
    {
        private static readonly IRuntimeSettingsProvider _provider = new RuntimeSettingsProvider();

        public RuntimeSettingsIntegrationTests() : base (_provider)
        {
        }

        [SetUp]
        public void TestSetup()
        {
            // ensure runtime settings file is found
            const string RuntimeSettingsFileName = "runtime.settings";
            var location = Path.Combine(TestContext.CurrentContext.TestDirectory, RuntimeSettingsFileName);
            Assert.IsTrue(File.Exists(location), $"Could not find '{RuntimeSettingsFileName}' here: {location}");
        }
    }
}