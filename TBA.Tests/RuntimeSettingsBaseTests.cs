using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests
{
    /// <summary>
    /// Base class for testing the runtime settings logic
    /// </summary>
    /// <remarks>
    /// <para>This is marked as abstract so that unit- and integration-tests can share the same common test logic</para>
    /// <para>If any implementation-specific tests are needed, then place them in the sub-class.</para>
    /// </remarks>
    public abstract class RuntimeSettingsBaseTests : TestBase
    {
        private readonly IRuntimeSettingsProvider _sut;

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="runtimeSettingsProvider">Runtime settings provider (mock) object</param>
        public RuntimeSettingsBaseTests(IRuntimeSettingsProvider runtimeSettingsProvider)
        {
            _sut = runtimeSettingsProvider;
        }

        [OneTimeSetUp]
        public void Test_BaselineAssertions_Success()
        {
            Assert.IsNotNull(_sut);
            Assert.IsNotNull(_sut.GetRuntimeSettings());
        }

        [Test]
        public void Test_SettingsAreValid_Success()
        {
            var settings = _sut.GetRuntimeSettings();
            var isValid = settings.ValidateSettings();
            Assert.IsTrue(isValid);
        }
    }
}