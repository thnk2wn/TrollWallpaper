using System;
using Core;
using EnsureThat;
using Quartz;
using WIO.Diagnostics;
using WIO.Imaging;
using WIO.Settings;

namespace WIO.Jobs
{
    internal class DownloadImagesJob : IJob
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public const string OutputPathKey = "OutputPath";

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                if (!AppSettings.Instance.CheckStatus()) return;

                if (!AppSettings.Instance.Search.Enabled)
                {
                    Logger.Info("Search and download images isn't enabled; exiting");
                    return;
                }

                // could also use context.MergedJobDataMap for job settings
                var outPath = AppSettings.ImagePath.FullName;
                Ensure.That(outPath, "outputPath").IsNotNullOrWhiteSpace();

                ImageCleanup.Execute();

                Logger.Info("Downloading images");
                    
                if (AppSettings.Instance.Search.Queries.Count > 5)
                    throw new InvalidOperationException("Please limit number of queries to 5");

                for (var q = 0; q < AppSettings.Instance.Search.Queries.Count; q++)
                {
                    var search = AppSettings.Instance.Search.Queries[q];

                    // try not to overwhelm system all at once, may draw too much attention
                    var delaySeconds = q*AppSettings.Instance.Search.DelaySecondsBetweenSearches;
                    var q1 = q;
                    Logger.Info("Starting batch {0} for term {1} w/delay seconds {2}", q1 + 1, search.Term, delaySeconds);
                    TaskDelayer.RunDelayed(delaySeconds * 1024, () =>
                    {
                        var fetcher = new SearchImageFetcher(outPath);
                        Logger.Info("Fetching images for {0}", search.Term);
                        fetcher.Fetch(search.Term, search.Options).Wait();
                        //TODO: capture time elapsed and # images downloaded and expose on fetcher class
                        return fetcher;
                    }).ContinueWith(t=> Logger.Info("Finished batch {0} for term {1} w/delay seconds {2}", q1 + 1, search.Term, delaySeconds));

                    if (!AppSettings.Instance.CheckStatus()) return;
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new JobExecutionException(ex);
            }
        }
    }
}
