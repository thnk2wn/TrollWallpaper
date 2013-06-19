namespace WIO.Settings
{
    public class SearchOptions
    {
        public SearchOptions()
        {
            this.Filters = "Size:Large";
            //this.Max = 100;
            this.Adult = "Moderate";
            this.MinHeight = 1024;
            this.MinWidth = 768;
        }

        //public int Max { get; set; }
        public string Filters { get; set; }
        public string Adult { get; set; }

        public int MinWidth { get; set; }
        public int MinHeight { get; set; }

        public override string ToString()
        {
            return string.Format("Filters: {0}, Adult: {1}", this.Filters, this.Adult);
        }
    }
}
