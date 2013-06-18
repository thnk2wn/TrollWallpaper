using System;
using System.Net;
using Newtonsoft.Json;

namespace WIO.Diagnostics
{
    internal class UserInfoFetcher
    {
        public static UserInfo Fetch()
        {
            return new UserInfo(
                username:string.Format(@"{0}\{1}", Environment.UserDomainName, Environment.UserName),
                geo:FetchGeo());
        }

        private static GeoInfo FetchGeo()
        {
            try
            {
                var webClient = new WebClient();
                var result = webClient.DownloadString("http://freegeoip.net/json/");
                var geoInfo = JsonConvert.DeserializeObject<GeoInfo>(result);
                return geoInfo;
            }
            catch(Exception ex)
            {
                LoggrNetLogger.LogError(ex, "Failed to resolve geo info");
            }

            return null;
        }
    }

    internal class UserInfo
    {
        public UserInfo(string username, GeoInfo geo)
        {
            this.Username = username;
            this.Geo = geo;
        }

        public string Username { get; private set; }
        public GeoInfo Geo { get; private set; }
    }

    internal class GeoInfo
    {
        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        public override string ToString()
        {
            return string.Format("{0},{1}", this.Latitude, this.Longitude);
        }
    }
}
