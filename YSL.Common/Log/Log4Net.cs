
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using log4net;

namespace YSL.Common.Log
{
    /// <summary>
    /// 记录日志服务实现类
    /// </summary>
    internal class Log4Net : ILogger
    {
        /// <summary>
        /// 配置文件对应Key
        /// </summary>
        private string ConfigKey;
        /// <summary>
        /// 日志操作对象
        /// </summary>
        private ILog Log;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="configName">配置文件对应Key</param>
        public Log4Net(string configName)
        {
            this.ConfigKey = configName;
            if (System.Web.HttpContext.Current == null)
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("Config/log4net.config"));
            else
                log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(System.Web.HttpContext.Current.Server.MapPath("~/Config/log4net.config")));
            Log = LogManager.GetLogger(this.ConfigKey);
        }
        public Log4Net(string configPath, string configName)
        {
            this.ConfigKey = configName;
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(configPath));
            Log = LogManager.GetLogger(this.ConfigKey);
        }

        /// <summary>
        /// 记录Info级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public virtual void Info(string message)
        {
            Log.Info(message.ToString());
        }
        /// <summary>
        /// 记录Info级别日志
        /// </summary>
        /// <param name="format">格式字符串</param>
        /// <param name="args">参数</param>
        public virtual void InfoFormat(string format, params string[] args)
        {
            Log.InfoFormat(format, args);
        }
        /// <summary>
        /// 记录Debug级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public virtual void Debug(string message)
        {
            Log.Debug(message.ToString());
        }
        /// <summary>
        /// 记录Debug级别日志
        /// </summary>
        /// <param name="format">格式字符串</param>
        /// <param name="args">参数</param>
        public virtual void DebugFormat(string format, params string[] args)
        {
            Log.DebugFormat(format, args);
        }
        /// <summary>
        /// 记录Warn级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public virtual void Warn(string message)
        {
            Log.Warn(message.ToString());
        }
        /// <summary>
        /// 记录Warn级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="ex">异常信息</param>
        public virtual void Warn(string message, Exception ex)
        {
            Log.Warn(message.ToString(), ex);
        }
        /// <summary>
        /// 记录Warn级别日志
        /// </summary>
        /// <param name="format">格式字符串</param>
        /// <param name="ex">异常信息</param>
        /// <param name="args">参数</param>
        public virtual void WarnFormat(string format, Exception ex, params string[] args)
        {
            string message = string.Format(format, args);
            Log.Warn(message, ex);
        }
        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="ex">异常信息</param>
        public virtual void Error(string message, Exception ex)
        {
            Log.Error(message, ex);
        }
        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public virtual void Error(string message)
        {
            Log.Error(message);
        }
        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="format"></param>
        /// <param name="ex">异常信息</param>
        /// <param name="args">格式参数</param>
        public virtual void ErrorFormat(string format, Exception ex, params string[] args)
        {
            string message = string.Format(format, args);
            Log.Error(message, ex);
        }
        /// <summary>
        /// 记录Fatal级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        public virtual void Fatal(string message)
        {
            Log.Fatal(message);
        }
        /// <summary>
        /// 记录Fatal级别日志
        /// </summary>
        /// <param name="format">日志内容</param>
        /// <param name="args">格式参数</param>
        public virtual void FatalFormat(string format, params string[] args)
        {
            string message = string.Format(format, args);
            Log.Fatal(message);
        }

        /// <summary>
        /// 记录Info级别日志
        /// </summary>
        /// <param name="content">日志内容对象</param>
        public void Info(LogContent content)
        {
            Log.Info(content);
        }
        /// <summary>
        /// 记录Debug级别日志
        /// </summary>
        /// <param name="content">日志内容对象</param>
        public void Debug(LogContent content)
        {
            Log.Debug(content);
        }
        /// <summary>
        /// 记录Warn级别日志
        /// </summary>
        /// <param name="content">日志内容对象</param>
        public void Warn(LogContent content)
        {
            Log.Warn(content);
        }
        /// <summary>
        /// 记录Fatal级别日志
        /// </summary>
        /// <param name="content">日志内容对象</param>
        public void Fatal(LogContent content)
        {
            Log.Fatal(content);
        }

        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="content">日志内容对象</param>
        public void Error(LogContent content)
        {
            Log.Error(content);
        }
    }
}
