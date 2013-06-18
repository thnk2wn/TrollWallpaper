using System;
using Quartz;
using WIO.Diagnostics;
using WIO.Settings;

namespace WIO.Jobs
{
    internal class LoadAppSettingsJob : IJob
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Logger.Info("Loading / refreshing app settings");
                AppSettings.Load().Wait(timeout:TimeSpan.FromSeconds(30));
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString());
                throw new JobExecutionException(ex);
            }
        }
    }
}
