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
        /// <param name="runtimeSettingsProvider">Runtime settings provider object -- real or mock</param>
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

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        [TestCase("not-a-real-url-at-all")]
        public void Test_SettingsInvalidApiUrl_Fail(string invalidApiUrl)
        {
            var settings = _sut.GetRuntimeSettings();
            settings.ApiBaseUrl = invalidApiUrl;
            Assert.Throws<SettingsFailureException>(() => settings.ValidateSettings());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Test_SettingsMissingAuthKey_Fail(string invalidKey)
        {
            var settings = _sut.GetRuntimeSettings();
            settings.AuthorizationHeaderKey = invalidKey;
            Assert.Throws<SettingsFailureException>(() => settings.ValidateSettings());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Test_SettingsMissingAuthValue_Fail(string invalidKey)
        {
            var settings = _sut.GetRuntimeSettings();
            settings.AuthorizationHeaderValue = invalidKey;
            Assert.Throws<SettingsFailureException>(() => settings.ValidateSettings());
        }

        [TestCase(int.MinValue)]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(9)]
        [TestCase(10)]
        [TestCase(int.MaxValue)]
        public void Test_SettingsThreadCountInvalid_Fail(int threadCount)
        {
            var settings = _sut.GetRuntimeSettings();
            settings.MaxThreadCount = threadCount;
            Assert.Throws<SettingsFailureException>(() => settings.ValidateSettings());
        }
    }
}