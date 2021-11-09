using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace TBA.Tests
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Assembly, AllowMultiple = true, Inherited = true)]
    public class RetryOnGatewayTimeout : NUnitAttribute, IRepeatTest
    {
        public int _tryCount;

        public RetryOnGatewayTimeout(int tryCount)
        {
            _tryCount = tryCount;
        }

        public TestCommand Wrap(TestCommand command)
        {
            throw new NotImplementedException();
        }
    }
}
