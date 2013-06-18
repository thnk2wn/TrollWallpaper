using System.Collections.Generic;

namespace WIO.Settings
{
    public class SearchOptions
    {
        public SearchOptions()
        {
            this.Filters = new Dictionary<string, string> { { "Size", "Large" } };
            this.Max = 100;
            this.Adult = "Moderate";
        }

        public int Max { get; set; }
        public Dictionary<string, string> Filters { get; set; }
        public string Adult { get; set; }
    }
}
