using System;
using System.Collections.Generic;
using System.Text;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 时间解析器
    /// </summary>
    public class TimeParser
    {
        /// <summary>
        /// 把秒转换成分钟
        /// </summary>
        /// <returns></returns>
        public static int SecondToMinute(int Second)
        {
            decimal mm = (decimal)((decimal)Second / (decimal)60);
            return Convert.ToInt32(Math.Ceiling(mm));
        }

        #region 返回某年某月最后一天
        /// <summary>
        /// 返回某年某月最后一天
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>日</returns>
        public static int GetMonthLastDate(int year, int month)
        {
            DateTime lastDay = new DateTime(year, month, new System.Globalization.GregorianCalendar().GetDaysInMonth(year, month));
            int Day = lastDay.Day;
            return Day;
        }
        #endregion

        #region 返回时间差
        public static string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            try
            {
                //TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                //TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                //TimeSpan ts = ts1.Subtract(ts2).Duration();
                TimeSpan ts = DateTime2 - DateTime1;
                if (ts.Days >= 1)
                {
                    dateDiff = DateTime1.Month.ToString() + "月" + DateTime1.Day.ToString() + "日";
                }
                else
                {
                    if (ts.Hours > 1)
                    {
                        dateDiff = ts.Hours.ToString() + "小时前";
                    }
                    else
                    {
                        dateDiff = ts.Minutes.ToString() + "分钟前";
                    }
                }
            }
            catch
            { }
            return dateDiff;
        }
        #endregion

        /// <summary>
        /// 转化为时间戳(精确到秒)
        /// </summary>
        /// <param name="datetime">时间</param>
        /// <returns></returns>
        public static string DateCuo(DateTime datetime)
        {
            DateTime timeStamp = new DateTime(1970, 1, 1);
            try
            {
                return ((datetime.AddHours(-8).Ticks - timeStamp.Ticks) / 10000000).ToString();
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 转化为时间戳(精确到毫秒)
        /// </summary>
        /// <param name="datetime">时间</param>
        /// <returns></returns>
        public static string DateCuoMsec(DateTime datetime)
        {
            var timeStamp = new DateTime(1970, 1, 1);
            return datetime < timeStamp ? null : ((datetime.AddHours(-8).Ticks - timeStamp.Ticks) / 10000).ToString();
        }

        //计算时间差(返回相差的秒数)
        public static int DateDiffSecond(DateTime DateTime1, DateTime DateTime2)
        {
            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts.Days * 24 * 60 * 60 + ts.Hours * 60 * 60 + ts.Minutes * 60 + ts.Seconds;
        }
    }
}
