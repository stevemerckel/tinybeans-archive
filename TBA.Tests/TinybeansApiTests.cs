using System;
using Moq;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests
{
    [TestFixture]
    [Ignore("not yet implemented")]
    public class TinybeansApiTests : TestBase
    {
        private readonly Mock<ITinybeansApiHelper> _sut;
        private ITinybeansJsonHelper _tinybeansJsonHelper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _tinybeansJsonHelper = new TinybeansJsonHelper(Logger);

            throw new NotImplementedException("This test class is not ready for primetime... yet.");
        }

        [Test]
        public void Test_JournalSummary_Success()
        {
            JournalSummary summary = null;
        }
    }
}