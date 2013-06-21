using System;
using System.IO;
using System.Linq;
using EnsureThat;
using Quartz;
using WIO.Diagnostics;
using WIO.Imaging;
using WIO.Settings;

namespace WIO.Jobs
{
    internal class SwitchWallpaperJob : IJob
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public const string OutputPathKey = "OutputPath";

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                //var path = context.MergedJobDataMap.GetString(OutputPathKey);
                var path = AppSettings.ImagePath.FullName;

                Ensure.That(path, "path").IsNotNullOrWhiteSpace();

                Logger.Troll("Enumerating files in {0}", path);
                var files = Directory.GetFiles(path, "*.jpg").ToList();

                if (!files.Any())
                {
                    Logger.Troll("No images yet; will try again later");
                    return;
                }

                Logger.Troll("Found {0} wallpaper images in {1}", files.Count, path);
                var r = new Random();
                var randFile = new FileInfo(files[r.Next(0, files.Count)]);

                Logger.Troll("Changing wallpaper to {0}", randFile.Name);
                Wallpaper.Set(randFile.FullName, Wallpaper.Style.Stretched);

                var meta = new MetadataManager().Get(randFile.FullName);
                var changedTo = (null != meta) ? meta.RemoteLocation + " via term " + meta.Term : randFile.Name;

                Logger.Troll("Changed wallpaper to {0}", changedTo);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new JobExecutionException(ex);
            }
        }
    }
}
