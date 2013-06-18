using System.Diagnostics;
using NLog;

namespace WIO.Diagnostics
{
    internal static class LoggerFactory
    {
        public static IAppLogger Create()
        {
            var loggerName = new StackFrame(1, false).GetMethod().DeclaringType.FullName;
            return Create(loggerName);
        }

        public static IAppLogger Create<T>()
        {
            return Create(typeof (T).FullName);
        }

        public static IAppLogger Create(string loggerName)
        {
            var log = (IAppLogger)LogManager.GetLogger(loggerName, typeof(AppLogger));
            return log;
        }
    }
}
