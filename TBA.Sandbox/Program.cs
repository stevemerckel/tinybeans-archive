using System;
using System.Linq;
using Ninject;
using TBA.Common;

namespace TBA.Sandbox
{
    class Program
    {
        private static readonly IKernel _kernel = new StandardKernel(new Ioc());

        static void Main(string[] args)
        {
            const int MonthCount = 1;
            var now = DateTime.Now.Date;
            var rangeStart = now.AddMonths(-1 * MonthCount);
            var rangeEnd = now;

            var logger = _kernel.Get<IAppLogger>();
            logger.Info("Hello World!");

            //Test01(logger, rangeStart, rangeEnd);
            Test02(logger, rangeStart, rangeEnd);


            Console.WriteLine("EOP");
            Console.ReadLine();
        }

        private static void Test02(IAppLogger logger, DateTime rangeStart, DateTime rangeEnd)
        {
            var jm = _kernel.Get<IJournalManager>();
            var journalSummaries = jm.GetJournalSummaries();
            logger.Info($"Found journals: Count = {journalSummaries?.Count ?? 0}");
            journalSummaries?.ForEach(x => logger.Info($"  {x}"));
            var journalId = journalSummaries.First().Id;
            var archives = jm.GetArchives(journalId.ToString(), rangeStart, rangeEnd);
            logger.Info($"Found {archives.Count} archives.  Writing to disk now...");
            jm.WriteArchivesToFileSystem(archives);
            logger.Info($"Wrote {archives.Count} archives to disk.");
        }

        private static void Test01(IAppLogger logger, DateTime rangeStart, DateTime rangeEnd)
        {
            var tbh = _kernel.Get<ITinybeansApiHelper>();
            var journalSummaries = tbh.GetJournalSummaries();
            logger.Info($"Found journals: Count = {journalSummaries?.Count ?? 0}");
            journalSummaries?.ForEach(x => logger.Info($"  {x}"));

            var journalId = journalSummaries.First().Id;
            var targetDate = new DateTime(2021, 1, 4);

            var dayEntries = tbh.GetByDate(targetDate, journalId);

            var monthEntries = tbh.GetEntriesByYearMonth(targetDate, journalId);

            var jm = _kernel.Get<IJournalManager>();
            var pool = jm.GetArchives(journalId.ToString(), rangeStart, rangeEnd);
        }
    }
}