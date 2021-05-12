using System;
using System.Collections.Generic;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests
{
    /// <summary>
    /// Abstract class encapsulating common tests of <see cref="ITinybeansApiHelper"/>
    /// </summary>
    public abstract class TinybeansApiBaseTests : TestBase
    {
        private readonly ITinybeansApiHelper _sut;
        
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="implementation">The real or mock implementation to test</param>
        public TinybeansApiBaseTests(ITinybeansApiHelper implementation)
        {
            _sut = implementation;
        }

        [Test]
        public void Test_JournalSummary_Success()
        {
            List<JournalSummary> summaries = null;
            Assert.DoesNotThrow(() => summaries = _sut.GetJournalSummaries());
            Assert.IsNotNull(summaries);
            Assert.IsTrue(summaries.Count > 0);
            summaries.ForEach(s =>
            {
                var utcEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                Assert.Multiple(() =>
                {
                    Assert.IsTrue(s.Id > 0);

                    Assert.IsFalse(string.IsNullOrWhiteSpace(s.Title));
                    Assert.IsFalse(string.IsNullOrWhiteSpace(s.Url));
                    Assert.IsTrue(s.Children.Count > 0, $"No children were found for journal ID '{s.Id}' -- was this expected??");

                    Assert.IsTrue(s.CreatedOnUtc >= new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));

                    // todo: implement conversion validation between ticks/ms/seconds and DateTime object.
                    //       but first need to find out what they are actually storing by uploading a test content and eval json response
                    // Assert.IsTrue(utcEpoch.Add(new DateTime(s.CreatedOnEpoch * 20, DateTimeKind.Utc) == s.CreatedOnUtc);
                });
            });
        }
    }
}