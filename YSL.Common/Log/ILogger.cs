using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YSL.Common.Log
{
    /// <summary>
    /// 日志服务接口
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// 记录Info级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        void Info(string message);
        /// <summary>
        /// 记录Info级别日志
        /// </summary>
        /// <param name="content">日志内容对象</param>
        void Info(LogContent content);
        /// <summary>
        /// 记录Info级别日志
        /// </summary>
        /// <param name="format">格式字符串</param>
        /// <param name="args">参数</param>
        void InfoFormat(string format, params string[] args);
        /// <summary>
        /// 记录Debug级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        void Debug(string message);
        /// <summary>
        /// 记录Debug级别日志
        /// </summary>
        /// <param name="content">日志内容对象</param>
        void Debug(LogContent content);
        /// <summary>
        /// 记录Debug级别日志
        /// </summary>
        /// <param name="format">格式字符串</param>
        /// <param name="args">参数</param>
        void DebugFormat(string format, params string[] args);
        /// <summary>
        /// 记录Warn级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        void Warn(string message);
        /// <summary>
        /// 记录Warn级别日志
        /// </summary>
        /// <param name="content">日志内容对象</param>
        void Warn(LogContent content);
        /// <summary>
        /// 记录Warn级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="ex">异常信息</param>
        void Warn(string message, Exception ex);
        /// <summary>
        /// 记录Warn级别日志
        /// </summary>
        /// <param name="message">格式字符串</param>
        /// <param name="ex">异常信息</param>
        /// <param name="args">参数</param>
        void WarnFormat(string message, Exception ex, params string[] args);
        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="ex">异常信息</param>
        void Error(string message, Exception ex);
        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        void Error(string message);
        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="content">日志内容对象</param>
        void Error(LogContent content);
        /// <summary>
        /// 记录Error级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="ex">异常信息</param>
        /// <param name="args">格式参数</param>
        void ErrorFormat(string message, Exception ex, params string[] args);
        /// <summary>
        /// 记录Fatal级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        void Fatal(string message);
        /// <summary>
        /// 记录Fatal级别日志
        /// </summary>
        /// <param name="content">日志内容对象</param>
        void Fatal(LogContent content);
        /// <summary>
        /// 记录Fatal级别日志
        /// </summary>
        /// <param name="message">日志内容</param>
        /// <param name="args">格式参数</param>
        void FatalFormat(string message, params string[] args);
    }
}
