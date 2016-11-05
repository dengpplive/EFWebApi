using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 身份证信息帮助类
    /// </summary>
    public class IdCardHelper
    {
        //位权值数组
        private static byte[] Weight = new byte[17];
        //身份证行政区划代码部分长度
        private static byte fPart = 6;
        //算法求模关键参数
        private static byte fMode = 11;
        //旧身份证长度
        private static byte oIdLen = 15;
        //新身份证长度
        private static byte nIdLen = 18;
        //新身份证年份标记值
        private static string yearFlag = "19";
        //校验字符串
        private static string checkCode = "10X98765432";
        //最小行政区划分代码
        private static int minCode = 110000;
        //最大行政区划分代码
        private static int maxCode = 820000;
        private static Random rand = new Random();

        /// <summary>
        /// 计算位权值数组
        /// </summary>
        private static void SetWBuffer()
        {
            for (int i = 0; i < Weight.Length; i++)
            {
                int k = (int)Math.Pow(2, (Weight.Length - i));
                Weight[i] = (byte)(k % fMode);
            }
        }

        /// <summary>
        /// 获取新身份证最后一位校验位
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        private static string GetCheckCode(string idCard)
        {
            if (string.IsNullOrEmpty(idCard)) return string.Empty;
            int sum = 0;
            //进行加权求和计算
            for (int i = 0; i < Weight.Length; i++)
            {
                sum += int.Parse(idCard.Substring(i, 1)) * Weight[i];
            }
            //求模运算得到模值
            byte mode = (byte)(sum % fMode);
            return checkCode.Substring(mode, 1);
        }

        /// <summary>
        /// 检查身份证长度是否合法
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        private static bool CheckLen(string idCard)
        {
            if (string.IsNullOrEmpty(idCard)) return false;
            if (idCard.Length == oIdLen || idCard.Length == nIdLen)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 验证是否是新身份证
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        private static bool IsNew(string idCard)
        {
            if (string.IsNullOrEmpty(idCard)) return false;
            if (idCard.Length == nIdLen)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取时间串
        /// </summary>
        /// <param name="idCard">身份证号码</param>
        /// <returns></returns>
        private static string GetDate(string idCard)
        {
            if (string.IsNullOrEmpty(idCard)) return string.Empty;
            string str = "";
            if (IsNew(idCard))
            {
                str = idCard.Substring(fPart, 8);
            }
            else
            {
                str = yearFlag + idCard.Substring(fPart, 6);
            }
            return str;
        }

        /// <summary>
        /// 检查时间是否合法
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        private static bool CheckDate(string idCard)
        {
            if (string.IsNullOrEmpty(idCard)) return false;
            //日期是否符合格式
            bool flag = false;
            string strDate = GetDate(idCard);

            int year = Convert.ToInt32(strDate.Substring(0, 4));
            int month = Convert.ToInt32(strDate.Substring(4, 2));
            int day = Convert.ToInt32(strDate.Substring(6, 2));

            //年份是否合法，本例暂定年份在1900-1999之间为合法年份
            if ((year > 1900) && (year < 2999))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

            //检查月份是否合法
            if ((month >= 1) && (month <= 12))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

            //检查天是否合法，本例以农历为准
            if ((day >= 1) && (day <= 30))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }
            return flag;
        }

        /// <summary>
        /// 根据年份和月份检查天是否合法
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <param name="day">天</param>
        /// <returns></returns>
        private static bool CheckDay(int year, int month, int day)
        {
            //是否是闰年
            bool rYearFlag = false;
            //天是否合法
            bool rDayFlag = false;
            if (((year % 4 == 0) && (year % 3200 != 0)) || (year % 400 == 0))
            {
                rYearFlag = true;
            }

            #region 检查天是否合法
            if (month == 2)
            {
                if (rYearFlag)
                {
                    if (day > 0 && day <= 29)
                    {
                        rDayFlag = true;
                    }
                }
                else
                {
                    if (day > 0 && day <= 28)
                    {
                        rDayFlag = true;
                    }
                }
            }
            else if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
            {
                if (day > 0 && day <= 31)
                {
                    rDayFlag = true;
                }
            }
            else
            {
                if (day > 0 && day <= 30)
                {
                    rDayFlag = true;
                }
            }
            #endregion

            return rDayFlag;
        }

        /// <summary>
        /// 检查身份证是否合法
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public static bool CheckCard(string idCard, out string msg)
        {
            if (string.IsNullOrEmpty(idCard))
            {
                msg = "身份证不能为空";
                return false;
            }
            //身份证是否合法标志
            bool flag = false;
            msg = string.Empty;
            SetWBuffer();
            if (!CheckLen(idCard))
            {
                msg = "身份证长度不符合要求";
                flag = false;
            }
            else
            {
                if (!CheckDate(idCard))
                {
                    msg = "身份证日期不符合要求";
                    flag = false;
                }
                else
                {
                    if (!IsNew(idCard))
                    {
                        idCard = GetNewIdCard(idCard);
                    }
                    string checkCode = GetCheckCode(idCard);
                    string lastCode = idCard.Substring(idCard.Length - 1, 1);
                    if (checkCode == lastCode)
                    {
                        flag = true;
                    }
                    else
                    {
                        msg = "身份证验证错误";
                        flag = false;
                    }
                }
            }
            return flag;
        }

        /// <summary>
        /// 旧身份证号码转换成新身份证号码
        /// </summary>
        /// <param name="oldIdCard">旧身份证号码</param>
        /// <returns>新身份证号码</returns>
        private static string GetNewIdCard(string oldIdCard)
        {
            if (string.IsNullOrEmpty(oldIdCard))
            {
                return string.Empty;
            }
            if (oldIdCard.Length == 15)
            {
                string newIdCard = oldIdCard.Substring(0, fPart);
                newIdCard += yearFlag;
                newIdCard += oldIdCard.Substring(fPart, 9);
                newIdCard += GetCheckCode(newIdCard);
                return newIdCard;
            }
            return string.Empty;
        }

        /// <summary>
        /// 新身份证号码转换成旧身份证号码
        /// </summary>
        /// <param name="newIdCard">新身份证号码</param>
        /// <returns>旧身份证号码</returns>
        private static string GetOldIdCard(string newIdCard)
        {
            if (string.IsNullOrEmpty(newIdCard))
            {
                return string.Empty;
            }
            if (newIdCard.Length == 18)
            {
                string oldIdCard = newIdCard.Substring(0, fPart);
                oldIdCard += newIdCard.Substring(8, 9);
                return oldIdCard;
            }
            return string.Empty;
        }


        /// <summary>
        /// 验证身份证是否有效
        /// </summary>
        /// <param name="strln"></param>
        /// <returns></returns>
        public static bool IsIdCard(string strln)
        {
            if (string.IsNullOrEmpty(strln))
            {
                return false;
            }
            if (strln.Length == 18)
            {
                bool check = IsIdCard18(strln);
                return check;
            }
            else if (strln.Length == 15)
            {
                bool check = IsIdCard15(strln);
                return check;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 验证输入字符串为18位的身份证号码
        /// </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        public static bool IsIdCard18(string strln)
        {
            if (string.IsNullOrEmpty(strln))
            {
                return false;
            }
            long n = 0;
            if (long.TryParse(strln.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(strln.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(strln.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = strln.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = strln.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != strln.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }
        /// <summary>
        /// 验证输入字符串为15位的身份证号码
        /// </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        public static bool IsIdCard15(string strln)
        {
            if (string.IsNullOrEmpty(strln))
            {
                return false;
            }
            long n = 0;
            if (long.TryParse(strln, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(strln.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = strln.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }
    }
}
