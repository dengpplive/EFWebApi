using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Framework.ScheduleTask
{
    /// <summary>
    /// 计划的类型接口
    /// </summary>
    public interface ISchedulerType
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        string Name
        {
            get;
        }
        /// <summary>
        /// 计划
        /// </summary>
        IScheduler Scheduler
        {
            get;
        }
        void Run();
    }
}
