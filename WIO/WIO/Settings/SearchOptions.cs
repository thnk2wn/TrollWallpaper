using EnsureThat;

namespace WIO.Settings
{
    public class SearchOptions
    {
        private int _max;

        public SearchOptions()
        {
            this.Filters = "Size:Large";
            this.Max = 300;
            this.Adult = "Moderate";
            this.MinHeight = 1920;
            this.MinWidth = 1080;
        }

        public int Max
        {
            get { return _max; }
            set { _max = Ensure.That(value, "Max").IsLte(300).Value; }
        }

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
