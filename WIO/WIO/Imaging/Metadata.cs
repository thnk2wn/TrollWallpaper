using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using WIO.Settings;

namespace WIO.Imaging
{
    public class Metadata
    {
        public Metadata()
        {
            Created = DateTime.Now;
        }

        public string RemoteLocation { get; set; }
        public string LocalLocation { get; set; }
        public string Term { get; set; }
        public DateTime Created { get; set; }
    }

    public class MetadataManager
    {
        private readonly List<Metadata> _metadataList = new List<Metadata>();
        private readonly object _lock = new object();

        public void Add(Metadata data)
        {
            _metadataList.Add(data);
        }

        public void Save()
        {
            lock (_lock)
            {
                var existing = Deserialize();
                existing.AddRange(_metadataList);
                var json = JsonConvert.SerializeObject(existing, Formatting.Indented);
                File.WriteAllText(Filename, json);
            }
        }

        private static List<Metadata> Deserialize()
        {
            if (!File.Exists(Filename)) return new List<Metadata>();
            return JsonConvert.DeserializeObject<List<Metadata>>(File.ReadAllText(Filename));
        }

        private static string Filename
        {
            get { return Path.Combine(AppSettings.ImagePath.FullName, "Metadata.json"); }
        }
    }
}
