using Moq;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests
{
    [TestFixture]
    public class RuntimeSettingsUnitTests : RuntimeSettingsBaseTests
    {
        private static readonly Mock<IRuntimeSettingsProvider> _provider = MakeMockProvider();

        public RuntimeSettingsUnitTests() : base(_provider.Object)
        {
        }

        /// <summary>
        /// Initializes the static mock with inner dependencies
        /// </summary>
        private static Mock<IRuntimeSettingsProvider> MakeMockProvider()
        {
            IRuntimeSettings fakeSettings = new RuntimeSettings
            {
                ApiBaseUrl = "https://fake.tinybeans.api.url.meh",
                AuthorizationHeaderKey = "fake-auth-key",
                AuthorizationHeaderValue = "fake-auth-value"
            };

            Mock<IRuntimeSettingsProvider> provider = new Mock<IRuntimeSettingsProvider>();
            provider
                .Setup(x => x.GetRuntimeSettings())
                .Returns(fakeSettings);

            return provider;
        }
    }
}