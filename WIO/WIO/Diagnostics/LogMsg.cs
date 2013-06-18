namespace WIO.Diagnostics
{
    internal class LogMsg
    {
        public string Text { get; set; }
        public string HtmlData { get; set; }
        public string Tags { get; set; }
        public string Source { get; set; }

        public LogMsg(string text, string htmlData = null, string tags = null, string source = null)
        {
            Text = text;
            HtmlData = htmlData;
            Tags = tags;
            Source = source;
        }
    }
}