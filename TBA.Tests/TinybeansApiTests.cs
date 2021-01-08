using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TBA.Common;

namespace TBA.Tests
{
    [TestFixture]
    [Ignore("not yet implemented")]
    public class TinybeansApiTests : TestBase
    {
        private Mock<ITinybeansApiHelper> _sut;
        private ITinybeansJsonHelper _tinybeansJsonHelper;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _tinybeansJsonHelper = new TinybeansJsonHelper(Logger);

            throw new NotImplementedException();
        }
    }
}
