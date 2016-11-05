using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YSL.Framework.ScheduleTask.Job;

namespace YSL.Framework.ScheduleTask.Trigger
{
    /// <summary>
    /// 例子 触发器
    /// </summary>
    public class AuthTriggerRunner : ISchedulerType
    {
        /// <summary>
        /// 类型名字
        /// </summary>
        public string Name
        {
            get { return GetType().Name; }
        }
        /// <summary>
        /// 计划
        /// </summary>
        public IScheduler Scheduler
        {
            get { return _Scheduler; }
        }
        private IScheduler _Scheduler = null;
        /// <summary>
        /// 启动运行计划
        /// </summary>
        public virtual void Run()
        {
            try
            {
                ISchedulerFactory sf = new StdSchedulerFactory();
                this._Scheduler = sf.GetScheduler();

                //..
                string strCronExpressionString = "";//Cron表达式

                JobDetail job = new JobDetail("job1", "group1", typeof(AuthTestingQuartzJob));
                CronTrigger trigger = new CronTrigger("trigger1", "group1", "job1", "group1");
                trigger.CronExpressionString = strCronExpressionString; //"0/20 * * * * ?";
                this._Scheduler.AddJob(job, true);
                DateTime ft = this._Scheduler.ScheduleJob(trigger);

                ////启动计划
                this._Scheduler.Start();
            }
            catch (Exception)
            {
                //停止计划
                this._Scheduler.Shutdown(true);
            }
        }
    }
}
