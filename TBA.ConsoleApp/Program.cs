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
            var journalIds = tbh.GetJournalIds();
            logger.Info($"Found journal IDs: Count = {journalIds?.Count ?? 0}");
            journalIds?.ForEach(x => logger.Info($"  {x}"));


            // init object
            // fetch journal ids --> grab first
            // grab entries by date using journal id above


            Console.WriteLine("EOP");
            Console.ReadLine();
        }
    }
}
