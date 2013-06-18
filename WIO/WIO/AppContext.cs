﻿using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using WIO.Core;
using WIO.Diagnostics;
using WIO.Jobs.Schedule;
using WIO.Settings;

namespace WIO
{
    internal class AppContext : ApplicationContext
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();
        private readonly JobScheduler _scheduler = new JobScheduler();

        public AppContext()
        {
            // configure default settings
            //var settingsJson = SettingsGenerator.Generate();

            AppSettings.Load().ContinueWith(AfterSettingsLoad);}

        private void AfterSettingsLoad(Task<AppSettings> task)
        {
            Debug.WriteLine(AppSettings.Instance.Search.Username);
            Logger.Info("Setting up scheduler");
            RegisterAppForWindowsStartup();
            _scheduler.Setup();
        }

        protected override void ExitThreadCore()
        {
            Logger.Info("In ExitThreadCore");
            base.ExitThreadCore();
            AppTeardown();
        }

        private void AppTeardown()
        {
            Logger.Info("Tearing down app");
            if (null != _scheduler)
            {
                Logger.Info("Disposing scheduler");
                _scheduler.Dispose();
            }

            RegisterAppForWindowsStartup();
        }

        private static void RegisterAppForWindowsStartup()
        {
            if (!DebugMode.IsDebugging)
                WindowsStartup.Register();
        }
    }
}