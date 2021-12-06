using Moq;
using System;
using TBA.Common;

namespace TBA.Tests
{
    /// <summary>
    /// Static collection of useful mocks
    /// </summary>
    public static class DefaultMocks
    {
        private static readonly Mock<IAppLogger> _mockLogger;
        private static readonly Mock<IRuntimeSettingsProvider> _mockRuntimeSettingsProvider;

        /// <summary>
        /// Default ctor
        /// </summary>
        static DefaultMocks()
        {
            //
            // init logger mock
            //

            _mockLogger = new Mock<IAppLogger>();

            _mockLogger
                .Setup(x => x.Critical(It.IsAny<string>()))
                .Callback<string>(message => Console.WriteLine($"[{nameof(IAppLogger.Critical)}]  {message}"));
            _mockLogger
                .Setup(x => x.Debug(It.IsAny<string>()))
                .Callback<string>(message => Console.WriteLine($"[{nameof(IAppLogger.Debug)}]  {message}"));
            _mockLogger
                .Setup(x => x.Error(It.IsAny<string>()))
                .Callback<string>(message => Console.WriteLine($"[{nameof(IAppLogger.Error)}]  {message}"));
            _mockLogger
                .Setup(x => x.Info(It.IsAny<string>()))
                .Callback<string>(message => Console.WriteLine($"[{nameof(IAppLogger.Info)}]  {message}"));
            _mockLogger
                .Setup(x => x.Verbose(It.IsAny<string>()))
                .Callback<string>(message => Console.WriteLine($"[{nameof(IAppLogger.Verbose)}]  {message}"));
            _mockLogger
                .Setup(x => x.Warn(It.IsAny<string>()))
                .Callback<string>(message => Console.WriteLine($"[{nameof(IAppLogger.Warn)}]  {message}"));

            //
            // init runtime settings provider mock
            //

            _mockRuntimeSettingsProvider = new Mock<IRuntimeSettingsProvider>();

            IRuntimeSettings fakeSettings = new RuntimeSettings
            {
                ApiBaseUrl = "https://fake.tinybeans.api.url.meh",
                AuthorizationHeaderKey = "fake-auth-key",
                AuthorizationHeaderValue = "fake-auth-value",
                MaxThreadCount = 2
            };

            _mockRuntimeSettingsProvider
                .Setup(x => x.GetRuntimeSettings())
                .Returns(fakeSettings);
        }

        /// <summary>
        /// Mocked logger object based on <see cref="IAppLogger"/>
        /// </summary>
        public static IAppLogger MockLogger => _mockLogger.Object;

        /// <summary>
        /// Mocked runtime settings provider based on <see cref="IRuntimeSettingsProvider"/>
        /// </summary>
        public static IRuntimeSettingsProvider MockRuntimeSettingsProvider => _mockRuntimeSettingsProvider.Object;
    }
}