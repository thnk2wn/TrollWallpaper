using Newtonsoft.Json;
using WIO.Settings;

namespace WIO.Core
{
    internal class SettingsGenerator
    {
        public static string Generate()
        {
            AppSettings.Instance.Search = new SearchSettings
                {
                    ApiKey = "RXExHlxpuLvlZU2raBU7J33ikghD7Rxn+Xx+uNY1kq8=",
                    Username = "thnk2wn2@gmail.com",
                    Queries =
                    {
                        new SearchQuery("Jessica Alba"),
                        new SearchQuery("Jessica Alba wallpaper"),
                        new SearchQuery("Jess Alba 2012")
                    },
                    ImageSearchUrl = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/v1"
                };

            AppSettings.Instance.Job = new JobSettings
            {
                DownloadImagesIntervalMinutes = 90,
                
                WallpaperIntervalMinutes = 60,
                WallpaperStartAfterMinutes = 10,

                LoadSettingsStartAfterMinutes = 5,
                LoadSettingsIntervalMinutes = 30,

                WindowsStartupDelayMinutes = 3
            };

            AppSettings.Instance.Log = new LogSettings
            {
                LoggrApiKey = "adaa9ad3d1f346e1ba31251c7bfa12bc",
                LoggrLogKey = "thnk2wn"
            };

            var jsonSettings = new JsonSerializerSettings {Formatting = Formatting.Indented};
            var json = JsonConvert.SerializeObject(AppSettings.Instance, jsonSettings);
            return json;
        }
    }
}
