namespace WIO.Settings
{
    public class LogSettings
    {
        public string LoggrApiKey { get; set; }
        public string LoggrLogKey { get; set; }
        public string LogglyInputKey { get; set; }

        public bool IsDebugEnabled { get; set; }
        public bool IsInfoEnabled { get; set; }
        public bool IsWarnEnabled { get; set; }
        public bool IsErrorEnabled { get; set; }
        public bool IsFatalEnabled { get; set; }
        public bool IsTrollEnabled { get; set; }
    }
}
