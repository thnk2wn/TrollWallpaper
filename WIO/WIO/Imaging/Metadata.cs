using System;
using Newtonsoft.Json;

namespace WIO.Imaging
{
    public class Metadata : ICloneable
    {
        [JsonProperty(PropertyName = "RL")]
        public string RemoteLocation { get; set; }

        [JsonProperty(PropertyName = "LL")]
        public string LocalLocation { get; set; }

        [JsonProperty(PropertyName = "T")]
        public string Term { get; set; }

        public object Clone()
        {
            return new Metadata
                {
                    RemoteLocation = this.RemoteLocation,
                    LocalLocation = this.LocalLocation,
                    Term = this.Term
                };
        }
    }
}
