
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YSL.Common.Log
{
    /// <summary>
    /// 日志内容数据模型
    /// </summary>
    public class LogContent
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="content">日志内容</param>
        /// <param name="userId">用户ID</param>
        /// <param name="userType">用户类型</param>
        /// <param name="userIP">用户IP</param>
        public LogContent(string content, int userId = 0, int userType = -1, string userIP = null)
        {
            this.Content = content;
            this.UserId = userId;
            this.UserType = userType;
            this.UserIP = userIP;
        }

        private string _Content = string.Empty;
        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content
        {
            get { return _Content; }
            set { _Content = value; }
        }

        private int _UserId = 0;
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId
        {
            get { return _UserId; }
            set { _UserId = value; }
        }

        private int _UserType = 0;
        /// <summary>
        /// 用户类型(0-前台用户，1-后台用户
        /// </summary>
        public int UserType
        {
            get { return _UserType; }
            set { _UserType = value; }
        }

        private string _UserIP = string.Empty;
        /// <summary>
        /// 用户IP
        /// </summary>
        public string UserIP
        {
            get { return _UserIP; }
            set { _UserIP = value; }
        }

        /// <summary>
        /// 返回当前日志模型内容
        /// </summary>
        /// <returns>日志模型内容Content</returns>
        public override string ToString()
        {
            return this.Content;
        }
    }
}
