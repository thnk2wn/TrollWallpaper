using System;
using System.IO;
using System.Linq;
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

                //TODO: how to decide when to re-execute search?
                var downloadImages = Directory.GetFiles(outPath, "*.jpg").Count() < 50;

                if (downloadImages)
                {
                    Logger.Info("Downloading images");
                    
                    if (AppSettings.Instance.Search.Queries.Count > 5)
                        throw new InvalidOperationException("Please limit number of queries to 5");

                    foreach (var search in AppSettings.Instance.Search.Queries)
                    {
                        var search1 = search;
                        Task.Factory.StartNew(() =>
                        {
                            var fetcher = new ImageFetcher(outPath);
                            Logger.Info("Fetching images for {0}", search1.Term);
                            fetcher.Fetch(search1.Term);
                            //TODO: options
                        }, TaskCreationOptions.LongRunning);
                    }
                }
                else
                    Logger.Info("Bypassing image downloading");
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new JobExecutionException(ex);
            }
            
        }
    }
}
