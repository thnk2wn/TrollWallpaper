using System.Threading.Tasks;
using System.Windows.Forms;
using WIO.Core;
using WIO.Diagnostics;
using WIO.Imaging;
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
            AppSettings.Load().ContinueWith(AfterSettingsLoad);
        }

        private void AfterSettingsLoad(Task<AppSettings> task)
        {
            if (AppSettings.Instance.Status == AppStatus.Disabled)
            {
                Application.Exit();
                return;
            }

            Logger.Info("Setting up scheduler");
            RegisterAppForWindowsStartup();
            _scheduler.Setup();
            ImageCleanup.Execute();
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

            ImageCleanup.Execute();

            RegisterAppForWindowsStartup();
        }

        private static void RegisterAppForWindowsStartup()
        {
            if (!DebugMode.IsDebugging)
                WindowsStartup.Register();
        }
    }
}
