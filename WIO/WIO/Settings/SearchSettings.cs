﻿using System.Collections.Generic;

namespace WIO.Settings
{
    public class SearchSettings
    {
        public SearchSettings()
        {
            this.DefaultOptions = new SearchOptions();
            this.Queries = new List<SearchQuery>();
            this.Timeout = 60;
        }

        public SearchOptions DefaultOptions { get; set; }

        public List<SearchQuery> Queries { get; set; }

        public string Username { get; set; }
        public string ApiKey { get; set; }
        public int Timeout { get; set; }

        public string ImageSearchUrl { get; set; }
    }
}
