using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using WIO.Settings;

namespace WIO.Imaging
{
    public class MetadataManager
    {
        private readonly List<Metadata> _metadataList = new List<Metadata>();
        private readonly object _lock = new object();

        public void Add(Metadata data)
        {
            var copy = (Metadata) data.Clone();
            copy.RemoteLocation = Encode(copy.RemoteLocation);
            copy.Term = Encode(copy.Term);
            _metadataList.Add(copy);
        }

        private static string Encode(string value)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(value));
        }

        private static string Decode(string value)
        {
            return Encoding.Unicode.GetString(Convert.FromBase64String(value));
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

        public Metadata Get(string localFilename)
        {
            var data = Deserialize();
            var meta = data.FirstOrDefault(x => x.LocalLocation == localFilename);

            if (null != meta)
            {
                meta.RemoteLocation = Decode(meta.RemoteLocation);
                meta.Term = Decode(meta.Term);
            }

            return meta;
        }

        private static List<Metadata> Deserialize()
        {
            if (!File.Exists(Filename)) return new List<Metadata>();
            
            using (var ms = new FileStream(Filename, FileMode.Open))
            using (var reader = new BsonReader(ms))
            {
                reader.ReadRootValueAsArray = true;
                var serializer = new JsonSerializer();
                var tempData = serializer.Deserialize<List<Metadata>>(reader);
                // cleanup by ignoring any old items in "index" where file no longer exists
                var data = tempData.Where(item => File.Exists(item.LocalLocation)).ToList();
                return data;
            }
        }

        private static string Filename
        {
            get { return Path.Combine(AppSettings.ImagePath.FullName, "Meta.idx"); }
        }
    }
}