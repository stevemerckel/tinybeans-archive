using Moq;
using System;
using TBA.Common;

namespace TBA.Tests
{
    /// <summary>
    /// Base class with common elements for testing
    /// </summary>
    public abstract class TestBase
    {
        private readonly Mock<IAppLogger> _mockLogger;

        /// <summary>
        /// Default ctor
        /// </summary>
        public TestBase()
        {
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
                .Setup(x => x.Warn(It.IsAny<string>()))
                .Callback<string>(message => Console.WriteLine($"[{nameof(IAppLogger.Warn)}]  {message}"));
        }

        /// <summary>
        /// Mocked logger object based on <seealso cref="IAppLogger"/>
        /// </summary>
        public IAppLogger Logger => _mockLogger.Object;
    }
}