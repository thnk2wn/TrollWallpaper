using Quartz;
using WIO.Diagnostics;
using WIO.Settings;

namespace WIO.Jobs.Schedule
{
    internal class WallpaperSchedule : ScheduleBase
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public WallpaperSchedule(IScheduler scheduler) : base(scheduler)
        {
        }

        public override void Setup()
        {
            Logger.Info("Setting up wallpaper job");
            var job = JobBuilder.Create<SwitchWallpaperJob>()
                .WithIdentity("wallpaper")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("wallpaperTrigger")
                .StartAt(AdjustOffset(DateBuilder.FutureDate(
                    AppSettings.Instance.Job.WallpaperStartAfterMinutes, IntervalUnit.Minute)))
                .WithSimpleSchedule(x =>
                    x.WithIntervalInMinutes(AppSettings.Instance.Job.WallpaperIntervalMinutes)
                    .RepeatForever())
                .Build();

            Scheduler.ScheduleJob(job, trigger);
            Logger.Info("Wallpaper job setup. Next fire time is {0}", GetNextFireTimeText(trigger));
        }
    }
}
