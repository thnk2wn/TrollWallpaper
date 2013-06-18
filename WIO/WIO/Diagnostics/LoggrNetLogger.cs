using System;
using System.Diagnostics;
using Loggr;
using WIO.Settings;

namespace WIO.Diagnostics
{
    // http://loggr.net/
    internal static class LoggrNetLogger
    {
        // consider using NLog agent: http://docs.loggr.net/agents/nlog
        // for now, want more control over interface with loggr.net

        public static void LogDebug(LogMsg msg, params object[] args)
        {
            Post("Debug", msg, args);
        }

        public static void LogInfo(LogMsg msg, params object[] args)
        {
            Post("Info", msg, args);
        }

        public static void LogInfo(string message, params object[] args)
        {
            LogInfo(new LogMsg(message), args);
        }

        public static void LogWarning(LogMsg msg, params object[] args)
        {
            Post("Warning", msg, args);
        }

        public static void LogTroll(LogMsg msg, params object[] args)
        {
            Post("Troll", msg, args);
        }

        public static void LogError(Exception ex, string message = null, params object[] args)
        {
            var resolvedMessage = (!string.IsNullOrWhiteSpace(message)) 
                ? string.Format(message, args) + "  ==>  " +ex.Message : ex.Message;
            LogError(new LogMsg(resolvedMessage, ex.ToString()));
        }

        public static void LogError(LogMsg msg, params object[] args)
        {
            Post("Error", msg, args);
        }

        private static void Post(string level, LogMsg msg, params object[] args)
        {
            var text = string.Format(msg.Text, args);

            try
            {
                var e = new Event
                    {
                        Text = text,
                        User = UserInfo.Username,
                        Source = msg.Source,
                        Data = msg.HtmlData,
                        DataType = DataType.html,
                        Geo = GeoInfo
                    };

                e.Tags.Add(level);

                if (!string.IsNullOrWhiteSpace(msg.Tags))
                    e.Tags.AddRange(msg.Tags.Split(' '));

                Client.Post(e);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error in remote logging: " + ex);
            }
            finally
            {
                Trace.WriteLine(text);
            }
            
        }

        private static LogClient _client;
        private static LogClient Client
        {
            get 
            { 
                return _client ?? (
                    _client = new LogClient(
                        LogKey: AppSettings.Instance.Log.LoggrLogKey, 
                        ApiKey:AppSettings.Instance.Log.LoggrApiKey)
                    ); 
            }
        }

        private static UserInfo _userInfo;
        private static UserInfo UserInfo
        {
            get { return _userInfo ?? (_userInfo = UserInfoFetcher.Fetch()); }
        }

        private static string GeoInfo
        {
            get { return null != UserInfo && null != UserInfo.Geo ? UserInfo.Geo.ToString() : null; }
        }
    }
}
