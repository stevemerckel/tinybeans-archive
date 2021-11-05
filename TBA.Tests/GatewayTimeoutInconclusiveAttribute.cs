using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal.Commands;

namespace TBA.Tests
{
    /// <summary>
    /// Decorates a test to indicate that HTTP 504 responses (Gateway Timeout) should be treated as "Inconclusive"
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Method|AttributeTargets.Assembly, AllowMultiple=true, Inherited=true)]
    public sealed class GatewayTimeoutInconclusiveAttribute : NUnitAttribute, IWrapSetUpTearDown
    {
        /// <inheritdoc />
        public TestCommand Wrap(TestCommand command)
        {
            var testResult = command.Test.MakeTestResult();
            if (testResult.ResultState == ResultState.Error || testResult.ResultState == ResultState.Failure)
            {
                var message = testResult.Message;
                if (!string.IsNullOrWhiteSpace(message))
                {
                    if (message.Contains("504") || message.Contains("Gateway Timeout", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // do... something
                    }
                }
            }

            return command;
        }
    }
}