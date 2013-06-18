using System;
using Quartz;
using WIO.Core;
using WIO.Settings;

namespace WIO.Jobs.Schedule
{
    internal abstract class ScheduleBase
    {
        private readonly IScheduler _scheduler;

        protected ScheduleBase(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public abstract void Setup();

        protected IScheduler Scheduler
        {
            get { return _scheduler; }
        }

        protected static DateTime? GetNextFireTime(ITrigger trigger)
        {
            var fireTimeUtc = trigger.GetNextFireTimeUtc();
            return fireTimeUtc.HasValue ? (DateTime?)fireTimeUtc.Value.LocalDateTime : null;
        }

        protected static string GetNextFireTimeText(ITrigger trigger)
        {
            var time = GetNextFireTime(trigger);
            return time.HasValue ? time.Value.ToString("G") : "Unknown";
        }

        protected static DateTimeOffset AdjustOffset(DateTimeOffset offset)
        {
            // if we're just starting up Windows, delay any normal offset a bit so we don't slow down windows startup
            if (WindowsStartup.UpTime.TotalMinutes <= AppSettings.Instance.Job.WallpaperStartAfterMinutes)
                offset = offset.AddMinutes(AppSettings.Instance.Job.WallpaperStartAfterMinutes);

            return offset;
        }
    }
}
