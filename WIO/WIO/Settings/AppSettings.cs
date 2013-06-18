using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WIO.Settings
{
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
                configData = await client.GetStringAsync(Properties.Settings.Default.RemoteConfigUri);
            }

            lock (SyncRoot)
            {
                _instance = JsonConvert.DeserializeObject<AppSettings>(configData);
            }

            return _instance;
        }

        [JsonIgnore]
        public static DirectoryInfo ImagePath
        {
            get
            {
                var di = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images"));
                if (!di.Exists) di.Create();
                return di;
            }
        }

        public SearchSettings Search { get; set; }

        public JobSettings Job { get; set; }

        public LogSettings Log { get; set; }
    }

    
}