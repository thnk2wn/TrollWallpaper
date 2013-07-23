using System;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WIO.Settings
{
    public enum AppStatus
    {
        Enabled,
        Paused,
        Disabled
    }

// ReSharper disable ClassCannotBeInstantiated
    public sealed class AppSettings
// ReSharper restore ClassCannotBeInstantiated
    {
        private static volatile AppSettings _instance;
        private static readonly object SyncRoot = new Object();

        private AppSettings()
        {
            this.Search = new SearchSettings();
            this.Job = new JobSettings();
            this.Log = new LogSettings();
        }

        public string ImageDeleteAfterTimespan { get; set; }

        public string WallpaperOverrideUrl { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AppStatus Status { get; set; }

        public static AppSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        _instance = new AppSettings();
                    }
                }
                return _instance;
            }
        }

        public static async Task<AppSettings> Load()
        {
            string configData;

            using (var client = new HttpClient())
            {
                configData = await client.GetStringAsync(ConfigurationManager.AppSettings["ConfigSource"]);

                var isJsonPlain = (configData.TrimStart().StartsWith("{"));
                if (!isJsonPlain)
                    configData = CryptoManager.Decrypt3DES(configData);
            }

            lock (SyncRoot)
            {
                _instance = JsonConvert.DeserializeObject<AppSettings>(configData);
            }

            return _instance;
        }

        // called manually as needed to encrypt contents
        public static string Protect(string remoteSource)
        {
            using (var client = new HttpClient())
            {
                var configData = client.GetStringAsync(remoteSource).Result;
                return CryptoManager.Encrypt3DES(configData);
            }
        }

        public bool CheckStatus()
        {
            if (Status == AppStatus.Disabled)
            {
                Application.Exit();
                return false;
            }

            return Status == AppStatus.Enabled;
        }

        [JsonIgnore]
        public static DirectoryInfo ImagePath
        {
            get
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WIO");
                var di = new DirectoryInfo(path);
                if (!di.Exists) di.Create();
                return di;
            }
        }

        public SearchSettings Search { get; set; }

        public JobSettings Job { get; set; }

        public LogSettings Log { get; set; }
    }

    
}