using NLog;

namespace WIO.Diagnostics
{
    public class AppLogger : Logger, IAppLogger
    {
        // TODO: log settings such as IsLogRemoteDebugEnabled, IsLogRemoteWarningEnabled...

        public new void Debug(string format, params object[] args)
        {
            base.Debug(format, args);
            //LoggrNetLogger.LogDebug(new LogMsg(string.Format(format, args)));
            LogglyLogger.Debug(format, args);
        }

        public new void Error(string format, params object[] args)
        {
            base.Error(format, args);
            LoggrNetLogger.LogError(new LogMsg(string.Format(format, args)));
        }

        public new void Fatal(string format, params object[] args)
        {
            base.Fatal(format, args);
            LoggrNetLogger.LogError(new LogMsg(string.Format(format, args)));
        }

        public new void Info(string format, params object[] args)
        {
            base.Info(format, args);
            //LoggrNetLogger.LogInfo(new LogMsg(string.Format(format, args)));
            LogglyLogger.Info(format, args);
        }

        public new void Trace(string format, params object[] args)
        {
            base.Trace(format, args);
            LoggrNetLogger.LogDebug(new LogMsg(string.Format(format, args)));
        }

        public new void Warn(string format, params object[] args)
        {
            base.Warn(format, args);
            LoggrNetLogger.LogWarning(new LogMsg(string.Format(format, args)));
        }

        public void Troll(string format, params object[] args)
        {
            LoggrNetLogger.LogTroll(new LogMsg(string.Format(format, args)));
        }
    }
}
