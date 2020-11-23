using Ninject;
using System;
using TBA.Common;

namespace TBA.ConsoleApp
{
    class Program
    {
        private static readonly IKernel _kernel = new StandardKernel(new Ioc());

        static void Main(string[] args)
        {
            // todo: adjust to read from commandline params
            var rangeStart = new DateTime(2017, 7, 14, 1, 1, 1, DateTimeKind.Local);

            // todo: adjust to read from param, or (if not given) add a few days to current date
            var rangeEnd = rangeStart.AddDays(60);

            // for-each over each day
            // todo: split range evenly into pools for multithread



            var logger = _kernel.Get<IAppLogger>();
            logger.Info("Hello World!");
            var tbh = _kernel.Get<ITinybeansApiHelper>();
            var journalSummaries = tbh.GetJournalSummaries();
            logger.Info($"Found journals: Count = {journalSummaries?.Count ?? 0}");
            journalSummaries?.ForEach(x => logger.Info($"  {x}"));

            var currentDate = rangeStart.Date;
            while (currentDate <= rangeEnd.Date)
            {
                // connect to api for this specific day

                // pull down all JSON entries
                // and hydrate to POCO

                // dump general data to console as proof

                // increment currentDate with a day
            }

            Console.WriteLine("EOP");
            Console.ReadLine();
        }
    }
}