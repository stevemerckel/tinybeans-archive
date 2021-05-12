using NUnit.Framework;
using System;

namespace TBA.Tests
{
    /// <summary>
    /// Base class with common elements for testing
    /// </summary>
    [TestFixture]
    public abstract class TestBase
    {
        /// <summary>
        /// Returns the path to the directory that is executing the test
        /// </summary>
        public string TestExecutionDirectory => TestContext.CurrentContext.TestDirectory;

        public TestBase()
        {
            // ensure calling class utilizes the proper decorator
            Type t = GetType();
            if (!t.IsDefined(typeof(TestFixtureAttribute), false))
            {
                throw new InvalidOperationException($"Type '{t.Name}' does not implement the expected attribute of '{nameof(TestFixtureAttribute)}'");
            }
        }
    }
}