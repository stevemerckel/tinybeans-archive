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
    /// <para>WARNING: Watch your use of the "_sut" object vs <see cref="GetRuntimeSettingsInstance"/>.  If you only need "read" access to the runtime settings, then you can use either.  If you are testing before/after changes, then use <see cref="GetRuntimeSettingsInstance"/> so that you don't affect concurrently running tests.</para>
    /// </remarks>
    public abstract class BaseRuntimeSettingsTests : TestBase
    {
        private readonly bool _isRuntimeSettingsProviderFake = false;
        private readonly IRuntimeSettingsProvider _sut;
        private const int ExpectedMinThreadCountAllowed = 1;
        private const int ExpectedMaxThreadCountAllowed = 8;

        /// <summary>
        /// Default ctor that uses a mocked <see cref="IRuntimeSettingsProvider"/> object
        /// </summary>
        public BaseRuntimeSettingsTests() : this(null, true)
        {
        }

        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="implementation">Runtime settings provider object</param>
        /// <param name="isProviderMock">Simple indication of whether the previous <see cref="IRuntimeSettingsProvider"/> object is a fake/mock implementation(<c>true</c>) or a real implementation (<c>false</c>)</param>
        public BaseRuntimeSettingsTests(IRuntimeSettingsProvider implementation, bool isProviderMock)
        {
            _sut = isProviderMock
                ? DefaultMocks.MockRuntimeSettingsProvider
                : implementation;

            _isRuntimeSettingsProviderFake = isProviderMock;
        }

        [OneTimeSetUp]
        public void Test_BaselineAssertions_Success()
        {
            Assert.IsNotNull(_sut);
            Assert.IsNotNull(_sut.GetRuntimeSettings());
            DefaultMocks.MockLogger.Info($"Finished '{nameof(Test_BaselineAssertions_Success)}' method:  {nameof(_isRuntimeSettingsProviderFake)} was {_isRuntimeSettingsProviderFake}");
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
        public void Test_ExpectedMaxIsNotLessThanMin_Success()
        {
            Assert.IsFalse(ExpectedMaxThreadCountAllowed < ExpectedMinThreadCountAllowed);
        }

        [TestCase(int.MinValue, ExpectedMinThreadCountAllowed)]
        [TestCase(-1, ExpectedMinThreadCountAllowed)]
        [TestCase(0, ExpectedMinThreadCountAllowed)]
        [TestCase(1, 1)]
        [TestCase(2, 2)]
        [TestCase(3, 3)]
        [TestCase(4, 4)]
        [TestCase(5, 5)]
        [TestCase(6, 6)]
        [TestCase(7, 7)]
        [TestCase(8, 8)]
        [TestCase(9, ExpectedMaxThreadCountAllowed)]
        [TestCase(int.MaxValue, ExpectedMaxThreadCountAllowed)]
        public void Test_EnsureThreadCountThresholds_Success(int attemptedThreadCount, int expectedThreadCount)
        {
            var rs = _sut.GetRuntimeSettings();
            rs.MaxThreadCount = attemptedThreadCount;
            Assert.AreEqual(expectedThreadCount, rs.MaxThreadCount);
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