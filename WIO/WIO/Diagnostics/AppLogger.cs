using NLog;
using WIO.Settings;

namespace WIO.Diagnostics
{
    public class AppLogger : Logger, IAppLogger
    {
        // TODO: log settings such as IsLogRemoteDebugEnabled, IsLogRemoteWarningEnabled...

        public new void Debug(string format, params object[] args)
        {
            if (!AppSettings.Instance.Log.IsDebugEnabled) return;
            base.Debug(format, args);
            //LoggrNetLogger.LogDebug(new LogMsg(string.Format(format, args)));
            LogglyLogger.Debug(format, args);
        }

        public new void Error(string format, params object[] args)
        {
            if (!AppSettings.Instance.Log.IsErrorEnabled) return;
            base.Error(format, args);
            LoggrNetLogger.LogError(new LogMsg(string.Format(format, args)));
        }

        public new void Fatal(string format, params object[] args)
        {
            if (!AppSettings.Instance.Log.IsFatalEnabled) return;
            base.Fatal(format, args);
            LoggrNetLogger.LogError(new LogMsg(string.Format(format, args)));
        }

        public new void Info(string format, params object[] args)
        {
            if (!AppSettings.Instance.Log.IsInfoEnabled) return;
            base.Info(format, args);
            //LoggrNetLogger.LogInfo(new LogMsg(string.Format(format, args)));
            LogglyLogger.Info(format, args);
        }

        public new void Trace(string format, params object[] args)
        {
            if (!AppSettings.Instance.Log.IsDebugEnabled) return;
            base.Trace(format, args);
            LoggrNetLogger.LogDebug(new LogMsg(string.Format(format, args)));
        }

        public new void Warn(string format, params object[] args)
        {
            if (!AppSettings.Instance.Log.IsWarnEnabled) return;
            base.Warn(format, args);
            LoggrNetLogger.LogWarning(new LogMsg(string.Format(format, args)));
        }

        public void Troll(string format, params object[] args)
        {
            if (!AppSettings.Instance.Log.IsTrollEnabled) return;
            LoggrNetLogger.LogTroll(new LogMsg(string.Format(format, args)));
        }
    }
}
