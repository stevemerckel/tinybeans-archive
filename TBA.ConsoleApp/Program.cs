using Ninject;
using System;
using System.Linq;
using TBA.Common;

namespace TBA.ConsoleApp
{
    class Program
    {
        private static readonly IKernel _kernel = new StandardKernel(new Ioc());

        static void Main(string[] args)
        {
            var rangeStart = new DateTime(2017, 7, 14, 1, 1, 1, DateTimeKind.Local);
            var rangeEnd = rangeStart.AddDays(60);

            var logger = _kernel.Get<IAppLogger>();
            logger.Info("Hello World!");
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

            Console.WriteLine("EOP");
            Console.ReadLine();
        }
    }
}