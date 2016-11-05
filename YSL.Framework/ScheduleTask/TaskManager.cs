using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Framework.ScheduleTask
{
    /// <summary>
    /// 任务计划的管理
    /// </summary>
    public class TaskManager
    {
        /// <summary>
        /// 开始启动任务注册
        /// </summary>
        public static void StartTask()
        {
            try
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                Type[] types = asm.GetTypes();
                types.Where(p => p.GetInterfaces().Select(p1 => p1 == typeof(ISchedulerType)).Count() > 0).ToList().ForEach(p =>
                {
                    //以”TriggerRunner“结尾的触发器类名
                    if (p.Name.EndsWith("TriggerRunner"))
                    {
                        //设置或者注册任务的触发
                        ISchedulerType sc = (ISchedulerType)ObjectUtils.InstantiateType(p);
                        sc.Run();
                        Console.WriteLine(p.Name + "定时任务已启动");
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartTask：启动失败", ex.Message);
            }
        }
    }
}
