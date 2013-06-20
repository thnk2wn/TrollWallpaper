using System;
using System.Threading.Tasks;
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
                // could also use context.MergedJobDataMap for job settings
                var outPath = AppSettings.ImagePath.FullName;
                Ensure.That(outPath, "outputPath").IsNotNullOrWhiteSpace();

                Logger.Info("Inspecting images in {0}", outPath);

                //var downloadImages = Directory.GetFiles(outPath, "*.jpg").Count() < 50;

                Logger.Info("Downloading images");
                    
                if (AppSettings.Instance.Search.Queries.Count > 5)
                    throw new InvalidOperationException("Please limit number of queries to 5");

                foreach (var search in AppSettings.Instance.Search.Queries)
                {
                    var search1 = search;
                    // consider not overwhelming system all at once, may draw too much attention
                    //Task.Factory.StartNew(() =>
                    //{
                        var fetcher = new ImageFetcher(outPath);
                        Logger.Info("Fetching images for {0}", search1.Term);
                        fetcher.Fetch(search1.Term, search1.Options);
                        //Task.Delay(TimeSpan.FromMinutes(1)).Wait();
                    //}, TaskCreationOptions.LongRunning);
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
