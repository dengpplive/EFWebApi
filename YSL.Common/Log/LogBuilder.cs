using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
namespace YSL.Common.Log
{
    /// <summary>
    /// 日志操作构造器
    /// </summary>
    public static class LogBuilder
    {
        /// <summary>
        /// 日志操作对象
        /// </summary>
        public static ILogger Log4Net { private set; get; }
        /// <summary>
        /// 获取动态记录日志的日志记录器操作对象
        /// </summary>
        public static NLog.Logger NLogger
        {
            get
            {
                //return GetCurrentClassLogger();
                return LogManager.GetCurrentClassLogger();
            }
        }

        /// <summary>
        /// 日志初始化方法
        /// </summary>
        /// <param name="configKey">配置文件中log4net/logger 中的name值</param>
        public static void InitLog4Net(string configKey)
        {
            Log4Net = new Log4Net(configKey);
        }

        /// <summary>
        /// 日志初始化方法
        /// </summary>
        /// <param name="configPath">配置文件路径</param>
        /// <param name="configKey">配置文件中log4net/logger 中的name值</param>
        public static void InitLog4Net(string configPath, string configKey)
        {
            Log4Net = new Log4Net(configPath, configKey);
        }

        /// <summary>
        /// 获取记录日志的NLog.Logger对象
        /// </summary>
        /// <returns></returns>
        private static NLog.Logger GetCurrentClassLogger()
        {
            return LogManager.GetLogger(GetClassFullName());
        }
        /// <summary>
        /// Gets the fully qualified name of the class invoking the LogManager, including the 
        /// namespace but not the assembly.    
        /// </summary>
        private static string GetClassFullName()
        {
            string className;
            Type declaringType;
            int framesToSkip = 2;
            do
            {
                StackFrame frame = new StackFrame(framesToSkip, true);
                MethodBase method = frame.GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    className = method.Name;
                    break;
                }
                framesToSkip++;
                className = declaringType.FullName;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return className;
        }
    }
}
