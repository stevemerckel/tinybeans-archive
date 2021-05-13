﻿using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TBA.Common;

namespace TBA.Tests
{
    /// <summary>
    /// Abstract class encapsulating common tests of <see cref="ITinybeansApiHelper"/>
    /// </summary>
    public abstract class BaseTinybeansApiTests : TestBase
    {
        private readonly ITinybeansApiHelper _sut;
        private readonly IAppLogger _logger = DefaultMocks.MockLogger;
        
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="implementation">The real or mock implementation to test</param>
        public BaseTinybeansApiTests(ITinybeansApiHelper implementation)
        {
            _sut = implementation;
        }

        [Test]
        public void Test_JournalSummaries_Success()
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

        [TestCase("2010-01-01", 0)]
        [TestCase("2018-09-19", 5)]
        public void Test_EntriesBySingleDate_Success(string dateString, int minimumExpectedEntryCount)
        {
            var journalId = _sut.GetJournalSummaries()?.Select(x => x.Id).FirstOrDefault() ?? null;
            if (journalId == null)
                throw new InvalidOperationException("No journal ID was found!  Do you have a journal setup with Tinybeans?");

            var target = DateTime.Parse(dateString);
            var entries = _sut.GetByDate(target, journalId.Value);
            ValidateReceivedEntries(entries, minimumExpectedEntryCount, target, journalId.Value);
        }

        [TestCase("2010-01-01", 0)]
        //[TestCase("2018-09-19", 5)]
        public void Test_EntriesByYearMonth_Success(string dateString, int minimumExpectedEntryCount)
        {
            var journalId = _sut.GetJournalSummaries()?.Select(x => x.Id).FirstOrDefault() ?? null;
            if (journalId == null)
                throw new InvalidOperationException("No journal ID was found!  Do you have a journal setup with Tinybeans?");

            var target = DateTime.Parse(dateString);
            var entries = _sut.GetEntriesByYearMonth(target, journalId.Value);
            ValidateReceivedEntries(entries, minimumExpectedEntryCount, target, journalId.Value);
        }

        /// <remarks>
        /// <para>This is kinda cheating, but the JSON payload for a "single date" is the same as "year month".</para>
        /// <para>So we're going to consolidate the validation in a single private method.</para>
        /// </remarks>
        private void ValidateReceivedEntries(List<ITinybeansEntry> entries, int minimumExpectedCount, DateTime target, long journalId)
        {
            Assert.IsTrue(entries.Count >= minimumExpectedCount);
            if (entries.Count > minimumExpectedCount)
                _logger.Warn($"We found {entries.Count} entries when the minimum expected count was {minimumExpectedCount} -- This may not be a concern, but logging just in case.  (journal ID = {journalId} --- target = {target.ToString("yyyy-MM-dd")})");

            // todo: build a few more checks
        }
    }
}