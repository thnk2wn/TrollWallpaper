using System;
using System.Linq;
using System.Reflection;
using Quartz;
using Quartz.Impl;
using WIO.Core;
using WIO.Diagnostics;

namespace WIO.Jobs.Schedule
{
    internal class JobScheduler : DisposableObject
    {
        private static readonly IAppLogger Logger = LoggerFactory.Create();

        public void Setup()
        {
            Logger.Info("Creating job scheduler");
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            Scheduler = schedFact.GetScheduler();
            Scheduler.Start();

            var types = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => x.BaseType == typeof(ScheduleBase)).ToList();
            Logger.Info("Found {0} schedules. Setting up each", types.Count);
            types.ForEach(t=> ((ScheduleBase) Activator.CreateInstance(t, Scheduler)).Setup());
            Logger.Debug("Schedules setup");
        }

        private IScheduler Scheduler { get; set; }
       
        protected override void DisposeManagedResources()
        {
            if (null != this.Scheduler)
                this.Scheduler.Shutdown(waitForJobsToComplete: false);
        }
    }
}
