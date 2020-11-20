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
            var logger = _kernel.Get<IAppLogger>();
            logger.Info("Hello World!");
            var tbh = _kernel.Get<ITinybeansApiHelper>();
            var journalSummaries = tbh.GetJournalSummaries();
            logger.Info($"Found journals: Count = {journalSummaries?.Count ?? 0}");
            journalSummaries?.ForEach(x => logger.Info($"  {x}"));


            // init object
            // fetch journal ids --> grab first
            // grab entries by date using journal id above


            Console.WriteLine("EOP");
            Console.ReadLine();
        }
    }
}
