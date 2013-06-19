using Loggly;
using WIO.Settings;

namespace WIO.Diagnostics
{
    public static class LogglyLogger
    {
        private static readonly Logger _logger;

        static LogglyLogger()
        {
            _logger = new Logger(AppSettings.Instance.Log.LogglyInputKey);
        }

        public static void Info(string message, params object[] args)
        {
            _logger.LogInfo(string.Format(message, args));
        }

        public static void Debug(string message, params object[] args)
        {
            _logger.LogVerbose(string.Format(message, args));
        }
    }
}
