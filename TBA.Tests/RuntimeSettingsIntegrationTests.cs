using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests
{
    [TestFixture]
    public class RuntimeSettingsIntegrationTests : RuntimeSettingsBaseTests
    {
        private static readonly IRuntimeSettingsProvider _provider = new RuntimeSettingsProvider();

        public RuntimeSettingsIntegrationTests() : base (_provider)
        {
        }
    }
}