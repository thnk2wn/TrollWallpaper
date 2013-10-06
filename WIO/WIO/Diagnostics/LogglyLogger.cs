using System.Diagnostics;
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
            var msg = string.Format(message, args);
            _logger.LogInfo(msg);
            Trace.WriteLine(msg);
        }

        public static void Debug(string message, params object[] args)
        {
            var msg = string.Format(message, args);
            _logger.LogVerbose(msg);
            Trace.WriteLine(msg);
        }
    }
}
