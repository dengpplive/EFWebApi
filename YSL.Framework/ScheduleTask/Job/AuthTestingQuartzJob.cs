using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YSL.Framework.ScheduleTask.Job
{
    //例子
    public class AuthTestingQuartzJob : IJob
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="context"></param>
        public void Execute(JobExecutionContext context)
        {

            try
            {
                //执行任务
                //.....
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
            }
        }
    }
}
