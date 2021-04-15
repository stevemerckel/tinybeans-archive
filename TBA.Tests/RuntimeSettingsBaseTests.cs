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
        private const int ExpectedMinThreadCountAllowed = 1;
        private const int ExpectedMaxThreadCountAllowed = 8;

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
            var settings = GetRuntimeSettingsInstance();
            settings.ApiBaseUrl = invalidApiUrl;
            Assert.Throws<SettingsFailureException>(() => settings.ValidateSettings());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Test_SettingsMissingAuthKey_Fail(string invalidKey)
        {
            var settings = GetRuntimeSettingsInstance();
            settings.AuthorizationHeaderKey = invalidKey;
            Assert.Throws<SettingsFailureException>(() => settings.ValidateSettings());
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void Test_SettingsMissingAuthValue_Fail(string invalidKey)
        {
            var settings = GetRuntimeSettingsInstance();
            settings.AuthorizationHeaderValue = invalidKey;
            Assert.Throws<SettingsFailureException>(() => settings.ValidateSettings());
        }

        [Test]
        public void Test_SettingsThreadCountOutsideRangeIsInvalid_Fail()
        {
            var settings = GetRuntimeSettingsInstance();
            var originalMaxThreadCount = settings.MaxThreadCount;

            Assert.Multiple(() =>
            {
                settings.MaxThreadCount = ExpectedMinThreadCountAllowed - 1;
                Assert.Throws<SettingsFailureException>(() => settings.ValidateSettings());
                settings.MaxThreadCount = ExpectedMaxThreadCountAllowed + 1;
                Assert.Throws<SettingsFailureException>(() => settings.ValidateSettings());
            });

            settings.MaxThreadCount = originalMaxThreadCount;
            Assert.AreEqual(originalMaxThreadCount, settings.MaxThreadCount);
        }

        [Test]
        public void Test_ExpectedMaxIsNotLessThanMin_Success()
        {
            Assert.IsFalse(ExpectedMaxThreadCountAllowed < ExpectedMinThreadCountAllowed);
        }

        [Test]
        public void Test_SettingsThreadCountInclusiveRangeIsValid_Success()
        {
            var settings = GetRuntimeSettingsInstance();
            var originalMaxThreadCount = settings.MaxThreadCount;

            Assert.Multiple(() =>
            {
                var targetThreadCountToTest = ExpectedMinThreadCountAllowed;
                while (true)
                {
                    // test
                    settings.MaxThreadCount = targetThreadCountToTest;
                    Assert.IsTrue(settings.ValidateSettings());

                    // increase count to test, but exit if we exceed the expected max allowed
                    targetThreadCountToTest++;
                    if (targetThreadCountToTest > ExpectedMaxThreadCountAllowed)
                        break;
                }
            });

            // ensure we can reset
            settings.MaxThreadCount = originalMaxThreadCount;
            Assert.AreEqual(originalMaxThreadCount, settings.MaxThreadCount);
        }

        /// <summary>
        /// <para>Use this to get a non-shared instance of <see cref="IRuntimeSettings"/> for testing.</para>
        /// <para>If you try accessing it from the received <see cref="IRuntimeSettingsProvider"/> object, testing changes will cause issues when tests run concurrently.</para>
        /// </summary>
        /// <remarks>
        /// As properties are added to <see cref="IRuntimeSettings"/> interface, also add the mappings in the returned object.
        /// </remarks>
        private IRuntimeSettings GetRuntimeSettingsInstance()
        {
            var original = _sut.GetRuntimeSettings();
            return new RuntimeSettings
            {
                ApiBaseUrl = original.ApiBaseUrl,
                AuthorizationHeaderKey = original.AuthorizationHeaderKey,
                AuthorizationHeaderValue = original.AuthorizationHeaderValue,
                MaxThreadCount = original.MaxThreadCount
            };
        }
    }
}