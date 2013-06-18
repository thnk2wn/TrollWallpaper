namespace WIO.Settings
{
    public class JobSettings
    {
        public int DownloadImagesIntervalMinutes { get; set; }

        public int WallpaperStartAfterMinutes { get; set; }
        public int WallpaperIntervalMinutes { get; set; }

        public int LoadSettingsStartAfterMinutes { get; set; }
        public int LoadSettingsIntervalMinutes { get; set; }

        public int WindowsStartupDelayMinutes { get; set; }
    }
}
