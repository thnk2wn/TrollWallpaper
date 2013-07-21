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
    internal class SwitchWallpaperJob : IJob
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public const string OutputPathKey = "OutputPath";

        public async void Execute(IJobExecutionContext context)
        {
            try
            {
                if (!AppSettings.Instance.CheckStatus()) return;

                //var path = context.MergedJobDataMap.GetString(OutputPathKey);
                var path = AppSettings.ImagePath.FullName;
                Ensure.That(path, "path").IsNotNullOrWhiteSpace();

                ImageCleanup.Execute();

                var imageFile = await GetWallpaperImageFile(path);
                if (null == imageFile) return;

                //TODO: setting for wallpaper style
                Logger.Troll("Changing wallpaper to {0}", imageFile.Name);
                Wallpaper.Set(imageFile.FullName, Wallpaper.Style.Centered);

                var meta = new MetadataManager().Get(imageFile.FullName);
                var changedTo = (null != meta) ? meta.RemoteLocation + " via term " + meta.Term : imageFile.Name;

                Logger.Troll("Changed wallpaper to {0}", changedTo);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new JobExecutionException(ex);
            }
        }

        private static async Task<FileInfo> GetWallpaperImageFile(string path)
        {
            if (!string.IsNullOrWhiteSpace(AppSettings.Instance.WallpaperOverrideUrl))
            {
                Logger.Troll("Using image override url {0}", AppSettings.Instance.WallpaperOverrideUrl);
                var uri = new Uri(AppSettings.Instance.WallpaperOverrideUrl);
                var file = uri.Segments[uri.Segments.Length - 1];
                var ext = file.Substring(file.LastIndexOf(".") + 1);
                var outFilename = new FileInfo(Path.Combine(AppSettings.ImagePath.FullName, "Override." + ext));
                await ImageDownloader.DownloadImage(AppSettings.Instance.WallpaperOverrideUrl, outFilename.FullName);
                return outFilename;
            }

            Logger.Troll("Enumerating files in {0}", path);
            var files = Directory.GetFiles(path, "*.jpg").ToList();

            if (!files.Any())
            {
                Logger.Troll("No images yet; will try again later");
                return null;
            }

            Logger.Troll("Found {0} wallpaper images in {1}", files.Count, path);
            var r = new Random();
            var randFile = new FileInfo(files[r.Next(0, files.Count)]);
            return randFile;
        }
    }
}
