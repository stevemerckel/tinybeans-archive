using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ninject;
using TBA.Common;

namespace TBA.Sandbox
{
    class Program
    {
        private static readonly IKernel _kernel = new StandardKernel(new Ioc());

        static async Task Main(string[] args)
        {
            Console.WriteLine("SOP");
            if (args.Any())
            {
                Console.WriteLine($"Received {args.Count()} arg(s)");
                var i = 0;
                foreach (var a in args)
                {
                    Console.WriteLine($"arg {i + 1}:\t{(string.IsNullOrWhiteSpace(a) ? "[NULL/EMPTY/WS]" : $"'{a}'")}");
                    i++;
                }
            }

            //WebClient301Test();
            //Console.WriteLine("EOP");
            //Console.ReadLine();
            //return;


            const int MonthCount = 60;
            var rangeStart = new DateTime(2017, 1, 1); // now.AddMonths(-1 * MonthCount);
            var rangeEnd = rangeStart.AddMonths(12); // now;

            var logger = _kernel.Get<IAppLogger>();
            logger.Info("Hello World!");

            //Test01(logger, rangeStart, rangeEnd);
            await Test02(logger, rangeStart, rangeEnd);

            Console.WriteLine();
            Console.WriteLine("EOP");
            Console.ReadLine();
        }

        private static void WebClient301Test()
        {
            const string OriginalUrl = @"https://tinybeans.com/pv/e/337074802/509a1b13-f380-41ed-9ed7-db992619072a-o.jpeg";
            const string ExpectedRedirectUrl = @"https://tinybeans-assets.s3.amazonaws.com/journals/917506/entries/2020/9/25/509a1b13-f380-41ed-9ed7-db992619072a-o.jpeg";

            var logger = _kernel.Get<IAppLogger>();
            logger.Debug($"Starting {nameof(WebClient301Test)}");
            WebClient wc = null;
            var localFileName = "test.jpg";
            try
            {
                wc = new WebClient();
                wc.DownloadFile(OriginalUrl, localFileName);
            }
            catch (WebException webEx)
            {

                var response = webEx.Response as HttpWebResponse;
                if (response == null)
                {

                }
                logger.Debug($"{nameof(WebException)} thrown -- details: {webEx}");

            }
            catch (Exception ex)
            {
                logger.Debug($"{nameof(Exception)} thrown -- details: {ex}");
            }
            finally
            {
                wc?.Dispose();
            }

        }

        private static async Task Test02(IAppLogger logger, DateTime rangeStart, DateTime rangeEnd)
        {
            var jm = _kernel.Get<IJournalManager>();
            jm.DownloadFailed += WriteFailed;
            jm.DownloadSucceeded += WriteSuccess;
            var runtimeSettings = _kernel.Get<IRuntimeSettingsProvider>().GetRuntimeSettings();
            var journalSummaries = await jm.GetJournalSummariesAsync();
            logger.Info($"Found journals: Count = {journalSummaries?.Count ?? 0}");
            journalSummaries?.ForEach(x => logger.Info($"  {x}"));
            var journalId = journalSummaries.First().Id;
            var archives = await jm.GetArchivesAsync(journalId.ToString(), rangeStart, rangeEnd);
            logger.Info($"Found {archives.Count} archives.  Using {runtimeSettings.MaxThreadCount} thread(s) to fetch + write to disk now...");
            await jm.WriteArchivesToFileSystemAsync(archives);
            logger.Info($"Wrote {archives.Count} archives to disk.");
            jm.DownloadFailed -= WriteFailed;
            jm.DownloadSucceeded -= WriteSuccess;
        }

        private static void WriteFailed(object sender, EntryDownloadInfo info)
        {
            Console.WriteLine($"[FAILED]  {info.ArchiveId}");
        }

        private static void WriteSuccess(object sender, EntryDownloadInfo info)
        {
            Console.WriteLine($"[SUCCESS]  {info.ArchiveId} at {info.LocalUri}");
        }

        private static async Task Test01(IAppLogger logger, DateTime rangeStart, DateTime rangeEnd)
        {
            var tbh = _kernel.Get<ITinybeansApiHelper>();
            var journalSummaries = await tbh.GetJournalSummariesAsync();
            logger.Info($"Found journals: Count = {journalSummaries?.Count ?? 0}");
            journalSummaries?.ForEach(x => logger.Info($"  {x}"));

            var journalId = journalSummaries.First().Id;
            var targetDate = new DateTime(2021, 1, 4);

            var dayEntries = await tbh.GetByDateAsync(targetDate, journalId);

            var monthEntries = await tbh.GetEntriesByYearMonthAsync(targetDate, journalId);

            var jm = _kernel.Get<IJournalManager>();
            var pool = await jm.GetArchivesAsync(journalId.ToString(), rangeStart, rangeEnd);
        }
    }
}