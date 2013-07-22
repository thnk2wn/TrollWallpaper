using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using EnsureThat;
using Microsoft.Win32;
using WIO.Diagnostics;

namespace WIO.Core
{
    internal static class WindowsStartup
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public static void Register()
        {
            SetStartup(true);
        }

        public static void Unregister()
        {
            SetStartup(false);
        }

        private static void SetStartup(bool enabled)
        {
            const string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            Logger.Debug("Opening registry key {0} for write", runKey);
            var key = Registry.CurrentUser.OpenSubKey(runKey, writable:true);
            Ensure.That(key, "key").IsNotNull();

            var name = Assembly.GetExecutingAssembly().GetName().Name;

            if (enabled)
            {
                Logger.Debug("Setting {0} to {1}", name, Application.ExecutablePath);
                key.SetValue(name, Application.ExecutablePath);
            }
            else
            {
                Logger.Debug("Deleting registry value {0}", name);
                key.DeleteValue(name);
            }

            Logger.Debug("Done");
        }

        public static TimeSpan UpTime
        {
            get
            {
                using (var uptime = new PerformanceCounter("System", "System Up Time"))
                {
                    uptime.NextValue();  // call this an extra time before reading its value
                    return TimeSpan.FromSeconds(uptime.NextValue());
                }
            }
        }
    }
}
