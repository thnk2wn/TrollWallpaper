namespace WIO.Settings
{
    public class SearchQuery
    {
        public SearchQuery(string term)
        {
            this.Term = term;
        }

        public string Term { get; set; }
        public SearchOptions Options { get; set; }

        public override string ToString()
        {
            return string.Format("Query: {0}", this.Term);
        }
    }
}
