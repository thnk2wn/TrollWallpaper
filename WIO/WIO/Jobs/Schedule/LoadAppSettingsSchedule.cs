using Quartz;
using WIO.Diagnostics;
using WIO.Settings;

namespace WIO.Jobs.Schedule
{
    internal class LoadAppSettingsSchedule : ScheduleBase
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public LoadAppSettingsSchedule(IScheduler scheduler) : base(scheduler)
        {
        }

        protected override void Setup()
        {
            Logger.Info("Setting up load app settings job");
            var job = JobBuilder.Create<LoadAppSettingsJob>()
                .WithIdentity("loadAppSettings")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("loadAppSettingsTrigger")
                .StartAt(AdjustOffset(DateBuilder.FutureDate(
                    AppSettings.Instance.Job.LoadSettingsStartAfterMinutes, IntervalUnit.Minute)))
                .WithSimpleSchedule(x =>
                    x.WithIntervalInMinutes(AppSettings.Instance.Job.LoadSettingsIntervalMinutes)
                    .RepeatForever())
                .Build();

            Scheduler.ScheduleJob(job, trigger);
            Logger.Info("Load app settings job setup. Next fire time is {0}", GetNextFireTimeText(trigger));
        }
    }
}
