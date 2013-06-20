using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using WIO.Settings;

namespace WIO.Imaging
{
    public class Metadata
    {
        public Metadata()
        {
            Created = DateTime.Now;
        }

        [JsonProperty(PropertyName = "RL")]
        public string RemoteLocation { get; set; }

        [JsonProperty(PropertyName = "LL")]
        public string LocalLocation { get; set; }

        [JsonProperty(PropertyName = "T")]
        public string Term { get; set; }

        [JsonProperty(PropertyName = "C")]
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

                using (var ms = new FileStream(Filename, FileMode.Create))
                using (var writer = new BsonWriter(ms))
                {
                  var serializer = new JsonSerializer();
                  serializer.Serialize(writer, existing);
                }
            }
        }

        private static List<Metadata> Deserialize()
        {
            if (!File.Exists(Filename)) return new List<Metadata>();
            
            using (var ms = new FileStream(Filename, FileMode.Open))
            using (var reader = new BsonReader(ms))
            {
                reader.ReadRootValueAsArray = true;
                var serializer = new JsonSerializer();
                var data = serializer.Deserialize<List<Metadata>>(reader);
                return data;
            }
        }

        private static string Filename
        {
            get { return Path.Combine(AppSettings.ImagePath.FullName, "Metadata.json"); }
        }
    }
}
