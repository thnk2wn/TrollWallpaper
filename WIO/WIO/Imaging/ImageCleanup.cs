using System;
using System.IO;
using System.Linq;
using WIO.Diagnostics;
using WIO.Settings;

namespace WIO.Imaging
{
    internal static class ImageCleanup
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public static void Execute()
        {
            Logger.Info("Attempting image cleanup using timespan value of {0}", AppSettings.Instance.ImageDeleteAfterTimespan);

            TimeSpan tsDeleteAfter;
            if (!TimeSpan.TryParse(AppSettings.Instance.ImageDeleteAfterTimespan, out tsDeleteAfter))
            {
                Logger.Error("Failed to parse delete after timespan value of: {0}", AppSettings.Instance.ImageDeleteAfterTimespan);
                return;
            }

            var toDelete = AppSettings.ImagePath.GetFiles("*.jpg", SearchOption.TopDirectoryOnly)
                                      .Where(f =>
                                             f.CreationTime.Add(tsDeleteAfter) < DateTime.Now)
                                      .ToList();
            Logger.Info("Found {0} image files to delete older than {1}", toDelete.Count, tsDeleteAfter.ToString());

            var deleteCount = 0;
            foreach (var fi in toDelete)
            {
                try
                {
                    fi.Delete();
                    deleteCount++;
                }
                catch (Exception ex)
                {
                    Logger.Error("failed to delete '{0}'; error was: {1}", fi.FullName, ex.Message);
                }
            }

            Logger.Info("Deleted {0} old images", deleteCount);
        }
    }
}
