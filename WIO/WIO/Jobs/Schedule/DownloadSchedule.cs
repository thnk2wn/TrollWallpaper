using Quartz;
using WIO.Diagnostics;
using WIO.Settings;

namespace WIO.Jobs.Schedule
{
    internal class DownloadSchedule : ScheduleBase
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public DownloadSchedule(IScheduler scheduler) : base(scheduler)
        {
        }

        public override void Setup()
        {
            Logger.Info("Setting up download images job");

            if (!AppSettings.Instance.Search.Enabled)
            {
                Logger.Info("Search and download images isn't enabled; exiting");
                return;
            }

            var job = JobBuilder.Create<DownloadImagesJob>()
                .WithIdentity("downloadImages")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("downloadImagesTrigger")
                .StartAt(AdjustOffset(DateBuilder.EvenMinuteDateAfterNow()))
                .WithSimpleSchedule(x =>
                    x.WithIntervalInMinutes(AppSettings.Instance.Job.DownloadImagesIntervalMinutes)
                    .RepeatForever())
                .Build();

            Scheduler.ScheduleJob(job, trigger);
            Logger.Info("Download job setup. Next fire time is {0}", GetNextFireTimeText(trigger));
        }
    }
}
