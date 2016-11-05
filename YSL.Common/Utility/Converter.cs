using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Xml;
using Microsoft.VisualBasic;
using YSL.Common.Extender;
namespace YSL.Common.Utility
{
    /// <summary>
    /// 基本转换
    /// </summary>
    public static class Converter
    {
        private static Regex RegexBr = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);
        public static Regex RegexFont = new Regex(@"<font color=" + "\".*?\"" + @">([\s\S]+?)</font>", GetRegexCompiledOptions());

        private static FileVersionInfo AssemblyFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        /// <summary>
        /// 得到正则编译参数设置
        /// </summary>
        /// <returns></returns>
        public static RegexOptions GetRegexCompiledOptions()
        {
#if NET1
            return RegexOptions.Compiled;
#else
            return RegexOptions.None;
#endif
        }
        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str)
        {
            return HttpUtility.UrlEncode(str);
        }
        /// <summary>
        /// 返回 URL 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="encode">文字编码方式gb2312，utf-8</param>
        /// <returns>编码结果</returns>
        public static string UrlEncode(string str, string encode)
        {
            //return HttpUtility.UrlEncode(str);
            byte[] bs = Encoding.GetEncoding(encode).GetBytes(str);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < bs.Length; i++)
            {
                if (bs[i] < 128)
                    sb.Append((char)bs[i]);
                else
                {
                    sb.Append("%" + bs[i++].ToString("x").PadLeft(2, '0'));
                    sb.Append("%" + bs[i].ToString("x").PadLeft(2, '0'));
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 返回 URL 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string UrlDecode(string str)
        {
            return HttpUtility.UrlDecode(str);
        }

        /// <summary>
        /// 返回Session的值
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string getSessionValue(string session)
        {
            if (System.Web.HttpContext.Current.Session[session] != null)
                return System.Web.HttpContext.Current.Session[session].ToString().Trim();
            else
                return "";
        }

        /// <summary>
        /// 返回Cookies的值
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string getCookiesValue(string cookies)
        {
            if (System.Web.HttpContext.Current.Request.Cookies[cookies] != null)
                return System.Web.HttpContext.Current.Request.Cookies[cookies].Value.ToString().Trim();
            else
                return "";
        }

        /// <summary>
        /// 返回QueryString的值
        /// </summary>
        /// <param name="querystring"></param>
        /// <returns></returns>
        public static string getQueryStringValue(string querystring)
        {
            if (System.Web.HttpContext.Current.Request.QueryString[querystring] != null)
                return System.Web.HttpContext.Current.Request.QueryString[querystring].ToString().Trim();
            else
                return "";
        }

        /// <summary>
        /// 返回Form的值
        /// </summary>
        /// <param name="querystring"></param>
        /// <returns></returns>
        public static string getFormValue(string querystring)
        {
            if (System.Web.HttpContext.Current.Request[querystring] != null)
                return System.Web.HttpContext.Current.Request[querystring].ToString().Trim();
            else
                return "";
        }

        public static bool IsCompriseStr(string str, string stringarray, string strsplit)
        {
            if (stringarray == "" || stringarray == null)
                return false;

            str = str.ToLower();
            string[] stringArray = SplitString(stringarray.ToLower(), strsplit);
            for (int i = 0; i < stringArray.Length; i++)
            {
                //string t1 = str;
                //string t2 = stringArray[i];
                if (str.IndexOf(stringArray[i]) > -1)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < stringArray.Length; i++)
            {
                if (caseInsensetive)
                {
                    if (strSearch.ToLower() == stringArray[i].ToLower())
                    {
                        return i;
                    }
                }
                else
                {
                    if (strSearch == stringArray[i])
                    {
                        return i;
                    }
                }

            }
            return -1;
        }


        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>		
        public static int GetInArrayID(string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0;
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">字符串数组</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string[] stringarray)
        {
            return InArray(str, stringarray, false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, SplitString(stringarray, ","), false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, SplitString(stringarray, strsplit), false);
        }


        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, strsplit), caseInsensetive);
        }

        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetArrayInArrayID(string[] strSearch, string[] stringArray, bool caseInsensetive)
        {
            for (int i = 0; i < strSearch.Length; i++)
            {
                if (caseInsensetive)
                {
                    strSearch[i] = strSearch[i].ToLower();
                }
                for (int j = 0; j < stringArray.Length; j++)
                {
                    if (caseInsensetive)
                    {
                        stringArray[j] = stringArray[j].ToLower();
                    }

                    if (strSearch[i] == stringArray[j])
                    {
                        return i * j;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// 判断指定字符串数组中有元素在另一字符串数组中
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool ArrayInArray(string[] strarraySearch, string[] stringArray, bool caseInsensetive)
        {
            return GetArrayInArrayID(strarraySearch, stringArray, caseInsensetive) >= 0;
        }

        /// <summary>
        /// 判断指定字符串数组中有元素在另一字符串数组中
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">字符串数组</param>
        /// <returns>判断结果</returns>
        public static bool ArrayInArray(string[] strarray, string[] stringarray)
        {
            return ArrayInArray(strarray, stringarray, false);
        }

        /// <summary>
        /// 判断指定字符串数组中有元素在另一字符串数组中
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <returns>判断结果</returns>
        public static bool ArrayInArray(string strarray, string stringarray)
        {
            return ArrayInArray(SplitString(strarray, ","), SplitString(stringarray, ","), false);
        }

        /// <summary>
        /// 判断指定字符串数组中有元素在另一字符串数组中
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <returns>判断结果</returns>
        public static bool ArrayInArray(string strarray, string stringarray, string strsplit)
        {
            return ArrayInArray(SplitString(strarray, strsplit), SplitString(stringarray, strsplit), false);
        }

        /// <summary>
        /// 判断指定字符串数组中有元素在另一字符串数组中
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool ArrayInArray(string strarray, string stringarray, string strsplit, bool caseInsensetive)
        {
            return ArrayInArray(SplitString(strarray, strsplit), SplitString(stringarray, strsplit), caseInsensetive);
        }

        /// <summary>
        /// 将该字符串从一组字符串中去除
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <returns>判断结果</returns>
        public static string OutArray(string str, string stringarray)
        {
            string strsplit = ",";
            string[] stringarrays = SplitString(stringarray, strsplit);
            for (int i = 0; i < stringarrays.Length; i++)
            {
                if (str == stringarrays[i])
                {
                    stringarrays[i] = null;
                }
            }

            StringBuilder res = new StringBuilder();
            for (int j = 0; j < stringarrays.Length; j++)
            {
                if (stringarrays[j] == null || stringarrays[j] == "") continue;
                if (res.Length > 0) res.Append(strsplit);
                res.Append(stringarrays[j]);
            }
            return res.ToString();
        }



        public static int getNumber(string Str)//将字符串转换为数字
        {

            System.Int32 inttmp;
            try
            {
                inttmp = System.Convert.ToInt32(Str);
                return inttmp;
            }
            catch
            {
                return 0;
            }

        }

        /// <summary>
        /// 判断给定的字符串数组(strNumber)中的数据是不是都为数值型
        /// </summary>
        /// <param name="strNumber">要确认的字符串数组</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsNumberArray(string[] strNumber)
        {
            if (strNumber == null)
            {
                return false;
            }
            if (strNumber.Length < 1)
            {
                return false;
            }
            foreach (string id in strNumber)
            {
                if (!IsNumber(id))
                {
                    return false;
                }
            }
            return true;

        }

        /// <summary>
        /// 取得时间标记，保证其不会重复性
        /// </summary>
        /// <returns></returns>
        public static string getTimeMark()
        {
            DateTime moment = DateTime.Now;
            long t = moment.ToFileTime();
            return t.ToString();
        }

        /// <summary>
        /// 判断是否数字
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static bool IsNumeric(string Str)
        {

            System.Int64 inttmp;
            try
            {
                inttmp = System.Convert.ToInt64(Str);
                return true;

            }
            catch
            {
                return false;
            }

        }
        /// <summary>
        /// 判断给定的字符串(strNumber)是否是数值型
        /// </summary>
        /// <param name="strNumber">要确认的字符串</param>
        /// <returns>是则返加true 不是则返回 false</returns>
        public static bool IsNumber(string strNumber)
        {
            return new Regex(@"^([0-9])[0-9]*(\.\w*)?$").IsMatch(strNumber);
            /*
            Regex objNotNumberPattern=new Regex("[^0-9.-]");
            Regex objTwoDotPattern=new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern=new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            string strValidRealPattern="^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            string strValidIntegerPattern="^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern =new Regex("(" + strValidRealPattern +")|(" + strValidIntegerPattern + ")");
            return !objNotNumberPattern.IsMatch(strNumber) &&
                !objTwoDotPattern.IsMatch(strNumber) &&
                !objTwoMinusPattern.IsMatch(strNumber) &&
                objNumberPattern.IsMatch(strNumber);
            */
        }
        /// <summary>
        /// string型转换为int型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int StrToInt(object strValue, int defValue)
        {
            if ((strValue == null) || (strValue.ToString() == string.Empty) || (strValue.ToString().Length > 10))
            {
                return defValue;
            }

            string val = strValue.ToString();
            string firstletter = val[0].ToString();

            if (val.Length == 10 && IsNumeric(firstletter) && int.Parse(firstletter) > 1)
            {
                return defValue;
            }
            else if (val.Length == 10 && !IsNumeric(firstletter))
            {
                return defValue;
            }

            int intValue = defValue;
            if (strValue != null)
            {
                bool IsInt = new Regex(@"^([-]|[0-9])[0-9]*$").IsMatch(strValue.ToString());
                if (IsInt)
                {
                    intValue = Convert.ToInt32(strValue);
                }
                else
                {
                    try
                    {
                        if (Convert.ToBoolean(strValue) == true) return 1;
                        else if (Convert.ToBoolean(strValue) == false) return defValue;
                    }
                    catch
                    {

                    }
                }
            }

            return intValue;
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的float类型结果</returns>
        public static float StrToFloat(object strValue, float defValue)
        {
            float intValue = defValue;
            if (strValue != null)
            {
                try
                {
                    bool IsFloat = new Regex(@"^([-]|[0-9])[0-9]*(\.\w*)?$").IsMatch(strValue.ToString());
                    if (IsFloat)
                    {
                        intValue = float.Parse(strValue.ToString());
                    }
                }
                catch
                {
                }
            }
            return intValue;
        }
        /// <summary>
        /// string型转换为short型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的short类型结果</returns>
        public static short StrToShort(object strValue, short defValue)
        {
            short shortValue = defValue;
            if (strValue != null)
            {
                try
                {
                    bool IsInt = new Regex(@"^([-]|[0-9])[0-9]*$").IsMatch(strValue.ToString());
                    if (IsInt)
                    {
                        shortValue = short.Parse(strValue.ToString());
                    }
                }
                catch
                {
                }
            }
            return shortValue;
        }
        /// <summary>
        /// string型转换为byte型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的short类型结果</returns>
        public static byte StrToByte(object strValue, byte defValue)
        {
            byte byteValue = defValue;
            if (strValue != null)
            {
                try
                {
                    bool IsInt = new Regex(@"^([-]|[0-9])[0-9]*$").IsMatch(strValue.ToString());
                    if (IsInt)
                    {
                        byteValue = byte.Parse(strValue.ToString());
                    }
                }
                catch
                {
                }
            }
            return byteValue;
        }

        /// <summary>
        /// string型转换为decimal型，保留两位小数
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的decimal类型结果</returns>
        public static decimal StrToDecimal(object strValue, decimal defValue)
        {
            return StrToDecimal(strValue, defValue, 2);
        }
        /// <summary>
        /// string型转换为decimal型，保留两位小数
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <param name="decimals">保留四舍五入的小数点位数</param>
        /// <returns>转换后的decimal类型结果</returns>
        public static decimal StrToDecimal(object strValue, decimal defValue, int decimals)
        {

            try
            {
                return decimal.Round(Convert.ToDecimal(string.Format("{0:0.000}", strValue)), decimals);
            }
            catch
            {
                return decimal.Round(Convert.ToDecimal(defValue), decimals);
            }
        }

        /// <summary>
        /// string型转换为double型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的double类型结果</returns>
        public static double StrToDouble(object strValue, double defValue)
        {
            if (strValue == null)
            {
                return defValue;
            }

            double intValue = defValue;
            if (strValue != null)
            {
                bool IsFloat = new Regex(@"^([-]|[0-9])[0-9]*(\.\w*)?$").IsMatch(strValue.ToString());
                if (IsFloat)
                {
                    try
                    {
                        intValue = double.Parse(strValue.ToString());
                    }
                    catch
                    { }
                }
            }
            return intValue;
        }
        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool StrToBool(object strValue, bool defValue)
        {
            try
            {
                if (IsNumber(strValue.ToString()))
                {
                    if (StrToInt(strValue, 0) == 1) return true;
                    else return false;
                }
                return Convert.ToBoolean(strValue);
            }
            catch
            {
                return defValue;
            }
        }
        /// <summary>
        /// string型转换为DateTime型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的DateTime类型结果</returns>
        public static DateTime StrToDateTime(object strValue, DateTime defValue)
        {
            try
            {
                if (IsDateTime(strValue.ToString()))
                {
                    return Convert.ToDateTime(strValue);
                }
                else return defValue;
            }
            catch
            {
                return defValue;
            }
        }
        /// <summary>
        /// 将long型数值转换为Int32类型
        /// </summary>
        /// <param name="objNum"></param>
        /// <returns></returns>
        public static int SafeInt32(object objNum)
        {
            if (objNum == null)
            {
                return 0;
            }
            string strNum = objNum.ToString();
            if (IsNumber(strNum))
            {

                if (strNum.ToString().Length > 9)
                {
                    return int.MaxValue;
                }
                return Int32.Parse(strNum);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 判断是否浮点
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static bool IsDouble(string Str)//判断是否浮点
        {

            System.Double inttmp;
            try
            {
                inttmp = System.Convert.ToDouble(Str);
                return true;

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断是否日期
        /// </summary>
        /// <param name="Str"></param>
        /// <returns></returns>
        public static bool IsDateTime(string Str)
        {

            System.DateTime Dtmp;

            try
            {
                Dtmp = System.Convert.ToDateTime(Str);
                return true;
            }
            catch
            {
                return false;

            }
        }

        #region 编译特殊字符
        /// <summary>
        /// 编译特殊字符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>返回编译过的字符串</returns>
        public static string editCharacter(string str)
        {
            //return editCharacter(str, webConfig.getConfig().text_replace);
            return editCharacter(str, false);
        }

        /// <summary>
        /// 编译特殊字符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="text_replace">是否替换过敏字符,true替换</param>
        /// <returns>返回编译过的字符串</returns>
        public static string editCharacter(string str, bool text_replace)
        {
            string strtemp = str.ToLower();
            //过滤SQL字符
            //strtemp = strtemp.Replace("?","");
            strtemp = strtemp.Replace("delcare", "delcar&#101;");
            strtemp = strtemp.Replace("and ", "&#097;nd ");
            strtemp = strtemp.Replace("select ", "sel&#101;ct ");
            strtemp = strtemp.Replace("join ", "jo&#105;n ");
            strtemp = strtemp.Replace("union ", "un&#105;on ");
            strtemp = strtemp.Replace("where ", "wh&#101;re ");
            strtemp = strtemp.Replace("insert ", "ins&#101;rt ");
            strtemp = strtemp.Replace("delete ", "del&#101;te ");
            strtemp = strtemp.Replace("update ", "up&#100;ate ");
            strtemp = strtemp.Replace("like ", "lik&#101; ");
            strtemp = strtemp.Replace("drop ", "dro&#112; ");
            strtemp = strtemp.Replace("create ", "cr&#101;ate ");
            strtemp = strtemp.Replace("modify ", "mod&#105;fy ");
            strtemp = strtemp.Replace("rename ", "ren&#097;me ");
            strtemp = strtemp.Replace("alter ", "alt&#101;r ");
            strtemp = strtemp.Replace("cast ", "ca&#115;t ");
            if (strtemp.Length != str.Length) str = strtemp;
            //过滤HTML
            //strtemp = strtemp.Replace(" ", "&nbsp;");
            str = str.Replace("&", "&amp;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("'", "''");
            str = str.Replace("*", "");
            str = str.Replace("\n", "<br />");
            str = str.Replace("\r\n", "<br />");
            str = str.Trim();
            //替换原字符串中的过敏字符
            //if (text_replace) str = this.BanWordFilter(str);
            return str;
        }
        #endregion

        #region 比较判断字符中是否含有特殊字符
        /// <summary>
        /// 比较判断字符中是否含有特殊字符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>含有返回FALSE，不含返回TRUE</returns>
        public static bool compareSameness(string str)
        {
            string strtemp = str;
            str = str.Replace("\\", "");
            str = str.Replace("^", "");
            str = str.Replace("/", "");
            str = str.Replace("#", "");
            str = str.Replace("$", "");
            str = str.Replace("%", "");
            str = str.Replace("|", "");
            str = str.Replace("+", "");
            str = str.Replace("&", "");
            str = str.Replace("<", "");
            str = str.Replace(">", "");
            str = str.Replace("'", "");
            str = str.Replace("*", "");
            str = str.Replace("\n", "");
            str = str.Replace("\r\n", "");
            if (strtemp.Length == str.Length)
                return true;
            else
                return false;
        }
        #endregion

        /// <summary>
        /// 清除给定字符串中的回车及换行符
        /// </summary>
        /// <param name="str">要清除的字符串</param>
        /// <returns>清除后返回的字符串</returns>
        public static string ClearBR(string str)
        {
            Regex r = null;
            Match m = null;

            r = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);
            for (m = r.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "");
            }


            return str;
        }

        /// <summary>
        /// 获得当前绝对路径
        /// </summary>
        /// <param name="strPath">指定的路径</param>
        /// <returns>绝对路径</returns>
        public static string GetMapPath(string strPath)
        {
            if (HttpContext.Current != null)
            {
                return HttpContext.Current.Server.MapPath(strPath);
            }
            else //非web程序引用
            {
                string ss = System.IO.Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, strPath);
                return ss;
            }
        }


        /// <summary>
        /// 返回文件是否存在
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否存在</returns>
        public static bool FileExists(string filename)
        {
            return System.IO.File.Exists(filename);
        }



        /// <summary>
        /// 以指定的ContentType输出指定文件文件
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="filename">输出的文件名</param>
        /// <param name="filetype">将文件输出时设置的ContentType</param>
        public static void ResponseFile(string filepath, string filename, string filetype)
        {
            Stream iStream = null;

            // 缓冲区为10k
            byte[] buffer = new Byte[10000];

            // 文件长度
            int length;

            // 需要读的数据长度
            long dataToRead;

            try
            {
                // 打开文件
                iStream = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);


                // 需要读的数据长度
                dataToRead = iStream.Length;

                HttpContext.Current.Response.ContentType = filetype;
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(filename.Trim()).Replace("+", " "));

                while (dataToRead > 0)
                {
                    // 检查客户端是否还处于连接状态
                    if (HttpContext.Current.Response.IsClientConnected)
                    {
                        length = iStream.Read(buffer, 0, 10000);
                        HttpContext.Current.Response.OutputStream.Write(buffer, 0, length);
                        HttpContext.Current.Response.Flush();
                        buffer = new Byte[10000];
                        dataToRead = dataToRead - length;
                    }
                    else
                    {
                        // 如果不再连接则跳出死循环
                        dataToRead = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                HttpContext.Current.Response.Write("Error : " + ex.Message);
            }
            finally
            {
                if (iStream != null)
                {
                    // 关闭文件
                    iStream.Close();
                }
            }
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImgFilename(string filename)
        {
            filename = filename.Trim();
            if (filename.EndsWith(".") || filename.IndexOf(".") == -1)
            {
                return false;
            }
            string extname = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
            return (extname == "jpg" || extname == "jpeg" || extname == "png" || extname == "bmp" || extname == "gif");
        }


        /// <summary>
        /// int型转换为string型
        /// </summary>
        /// <returns>转换后的string类型结果</returns>
        public static string IntToStr(int intValue)
        {
            //
            return Convert.ToString(intValue);
        }

        /// <summary>
        /// 转换为静态html
        /// </summary>
        public static void transHtml(string path, string outpath)
        {
            Page page = new Page();
            StringWriter writer = new StringWriter();
            page.Server.Execute(path, writer);
            FileStream fs;
            if (File.Exists(page.Server.MapPath("") + "\\" + outpath))
            {
                File.Delete(page.Server.MapPath("") + "\\" + outpath);
                fs = File.Create(page.Server.MapPath("") + "\\" + outpath);
            }
            else
            {
                fs = File.Create(page.Server.MapPath("") + "\\" + outpath);
            }
            byte[] bt = Encoding.Default.GetBytes(writer.ToString());
            fs.Write(bt, 0, bt.Length);
            fs.Close();
        }

        #region 截取需要显示的长度(<font color="red">注意：长度是以byte为单位的，一个汉字是2个byte</font>)
        /// <summary>
        /// 截取需要显示的长度(<font color="red">注意：长度是以byte为单位的，一个汉字是2个byte</font>)
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="len">这里的长度以byte为度量单位,一个汉字是2个byte</param>
        /// <returns></returns>
        public static string getLimitLengthString(string str, int len)
        {
            return getLimitLengthString(str, len, "");
        }
        /// <summary>
        /// 截取需要显示的长度(<font color="red">注意：长度是以byte为单位的，一个汉字是2个byte</font>)
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="len">这里的长度以byte为度量单位,一个汉字是2个byte</param>
        /// <returns></returns>
        public static string getLimitLengthString(object str, int len)
        {
            return getLimitLengthString(str.ToString(), len, "");
        }
        /// <summary>
        /// 截取需要显示的长度(<font color="red">注意：长度是以byte为单位的，一个汉字是2个byte</font>)
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="len">这里的长度以byte为度量单位,一个汉字是2个byte</param>
        /// <returns></returns>
        public static string getLimitLengthString(object str, int len, string suffix)
        {
            return getLimitLengthString(str.ToString(), len, suffix);
        }
        /// <summary>
        /// 截取需要显示的长度(<font color="red">注意：长度是以byte为单位的，一个汉字是2个byte</font>) 修补了最后一个br未结束
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="len">这里的长度以byte为度量单位,一个汉字是2个byte</param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string getLimitLengthString(string str, int len, string suffix)
        {
            string strs = str as string;
            int counterOfDoubleByte = 0;
            byte[] bytestr = Encoding.Default.GetBytes(strs);
            if (bytestr.Length <= len) return strs;
            for (int i = 0; i < len; i++)
            {
                //特别字符或英文字母
                if (bytestr[i] > 26 && bytestr[i] <= 126)
                    counterOfDoubleByte++;
            }
            string temp;
            if (counterOfDoubleByte % 2 == 0)
                temp = Encoding.Default.GetString(bytestr, 0, len) + suffix;
            else
                temp = Encoding.Default.GetString(bytestr, 0, (len - 1)) + suffix;
            //修改<  >未完成的情况
            int docstart = temp.LastIndexOf('<');
            int docend = temp.LastIndexOf('>');

            if (docstart > docend)
            {
                temp = getLimitLengthString(temp, docstart, suffix);
            }
            return temp;
        }

        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            return GetSubString(str, startIndex, length, "");
        }

        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex)
        {
            return GetSubString(str, startIndex, (str.Length - startIndex), "");
        }


        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result);  //返回长度为44字节的字符串
        }


        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }


        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            string myResult = p_SrcString;

            //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
            if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") ||
                System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
            {
                //当截取的起始位置超出字段串长度时
                if (p_StartIndex >= p_SrcString.Length)
                {
                    return "";
                }
                else
                {
                    return p_SrcString.Substring(p_StartIndex,
                                                   ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                }
            }


            if (p_Length >= 0)
            {
                byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                //当字符串长度大于起始位置
                if (bsSrcString.Length > p_StartIndex)
                {
                    int p_EndIndex = bsSrcString.Length;

                    //当要截取的长度在字符串的有效长度范围内
                    if (bsSrcString.Length > (p_StartIndex + p_Length))
                    {
                        p_EndIndex = p_Length + p_StartIndex;
                    }
                    else
                    {   //当不在有效范围内时,只取到字符串的结尾

                        p_Length = bsSrcString.Length - p_StartIndex;
                        p_TailString = "";
                    }



                    int nRealLength = p_Length;
                    int[] anResultFlag = new int[p_Length];
                    byte[] bsResult = null;

                    int nFlag = 0;
                    for (int i = p_StartIndex; i < p_EndIndex; i++)
                    {

                        if (bsSrcString[i] > 127)
                        {
                            nFlag++;
                            if (nFlag == 3)
                            {
                                nFlag = 1;
                            }
                        }
                        else
                        {
                            nFlag = 0;
                        }

                        anResultFlag[i] = nFlag;
                    }

                    if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                    {
                        nRealLength = p_Length + 1;
                    }

                    bsResult = new byte[nRealLength];

                    Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);

                    myResult = Encoding.Default.GetString(bsResult);

                    myResult = myResult + p_TailString;
                }
            }

            return myResult;
        }
        #endregion

        /// <summary>
        /// 保存br
        /// </summary>
        /// <param name="fString"></param>
        /// <returns></returns>
        public static string ubbEncode(string fString)
        {
            string strtmp = fString;
            if (fString != null || fString.Length != 0)
            {

                //替换回车
                char ch;
                ch = (char)32;
                strtmp = strtmp.Replace(ch.ToString(), "&nbsp;");
                ch = (char)34;
                strtmp = strtmp.Replace(ch.ToString(), "&quot;");
                ch = (char)39;
                strtmp = strtmp.Replace(ch.ToString(), "&#39;");
                //ch = (char)13;
                //strtmp = strtmp.Replace(ch.ToString(), "<br />");
                //ch = (char)10;
                //strtmp = strtmp.Replace(ch.ToString(), "<br />");
                //strtmp = strtmp.Replace("\r\n", "<br />");
                strtmp = strtmp.Replace("\n", "<br />");
                strtmp = strtmp.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">");
                strtmp = Regex.Replace(strtmp, @"<br[^>]+>", @"[br /]", RegexOptions.IgnoreCase);
                //替换<>之间的值
                strtmp = Regex.Replace(strtmp, @"<([^>]+)>", @"", RegexOptions.IgnoreCase);
                //替换原字符串中的过敏字符
                //if (sconfig.text_replace) strtmp = BanWordFilter(strtmp);
                //strtmp = EncodeHtml(strtmp);

            }
            return strtmp.Replace("[br /]", "<br />");
        }

        public static string ubbuncode(string fString)
        {
            string strtmp = fString;
            if (fString != null || fString.Length != 0)
            {
                //替换回车
                strtmp = strtmp.Replace("<br />", "\n");
                strtmp = strtmp.Replace("&quot;", "\"");
                strtmp = strtmp.Replace("&nbsp;", " ");
                //替换<>之间的值
                //strtmp = Regex.Replace(strtmp, @"<[^>]+>", @"", RegexOptions.IgnoreCase);
                //strtmp = strtmp.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">");

            }
            return strtmp;
        }

        public static string htmlEncodeNotNewline(string fString)
        {
            string strtmp = fString;
            if (fString != null || fString.Length != 0)
            {
                //替换回车
                //strtmp = strtmp.Replace("\n", "&nbsp;");
                //strtmp = strtmp.Replace("\r\n", "&nbsp;");
                //strtmp = strtmp.Replace("<BR />", "&nbsp;");
                //替换<>之间的值
                strtmp = strtmp.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">");

                //取出文章中的图片提取到前面显示
                string re = @"(src=\S+\.{1}(gif|jpg|png|bmp)(""|\')?)";
                MatchCollection mat = Regex.Matches(strtmp, re, RegexOptions.IgnoreCase);
                //不替换换行
                strtmp = Regex.Replace(strtmp, @"<br(.[^>]+)>", @"[br /]", RegexOptions.IgnoreCase);
                strtmp = Regex.Replace(strtmp, @"<[^>]+>", @"", RegexOptions.IgnoreCase);

                if (mat.Count > 0)
                {
                    strtmp = "<div style=\"text-align:center\"><img " + mat[0].Value + " onclick=\"ImgOpen(this);\" onload=\"ImgSizeAuto(this);\" /></div><br>" + strtmp;
                }


            }
            return strtmp.Replace("[br /]", "<br />");
        }

        /// <summary>
        /// 解码HTML字符集，不换行模式
        /// </summary>
        /// <param name="fString"></param>
        /// <returns></returns>
        public static string htmlUncodeNotNewline(string fString)
        {
            string strtmp = fString;
            if (fString != null || fString.Length != 0)
            {
                //替换回车
                strtmp = strtmp.Replace("\n", "&nbsp;");
                strtmp = strtmp.Replace("\r\n", "&nbsp;");
                strtmp = strtmp.Replace("<BR />", "&nbsp;");
                strtmp = strtmp.Replace("<br />", "&nbsp;");
                strtmp = strtmp.Replace("<br/>", "&nbsp;");
                strtmp = strtmp.Replace("<br>", "&nbsp;");
                //替换<>之间的值
                //strtmp = strtmp.Replace("&amp;", "&").Replace("&lt;", "<").Replace("&gt;", ">");
                strtmp = Regex.Replace(strtmp, @"<[^>]+>", @"", RegexOptions.IgnoreCase);
            }
            return strtmp;
        }

        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string HTML)
        {
            HTML = HTML.Replace("&lt;", "<");
            HTML = HTML.Replace("&gt;", ">");
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(@"</?(?!br|/?p|img)[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return regEx.Replace(HTML, "");
        }

        public static string HTMLEncode(string fString)
        {
            Regex r;
            Match m;

            //string strtmp = HtmlDecode(fString);
            string strtmp = fString;
            if (strtmp != null || strtmp.Length != 0)
            {

                //替换回车
                /*char ch;
                ch=(char)32;
                strtmp = strtmp.Replace(ch.ToString(), "&nbsp;");
                ch=(char)34;
                strtmp = strtmp.Replace(ch.ToString(), "&quot;");
                ch=(char)39;
                strtmp = strtmp.Replace(ch.ToString(), "&#39;");
                ch=(char)13;
                strtmp = strtmp.Replace(ch.ToString(), "");
                ch=(char)10;
                 */
                //替换<>之间的值
                /*strtmp = strtmp.ToLower().Replace(Convert.ToString((char)39), "\"").Replace("'", "''").Replace("<script>", "[script]").Replace("</script>", "[/script]");
                strtmp = strtmp.ToLower().Replace("<iframe", "[iframe").Replace("</iframe>", "[/iframe]");
                strtmp = strtmp.Replace("&amp;", "&").Replace("<img ", "<img onclick=\"ImgOpen(this);\" onload=\"ImgSizeAuto(this);\" ");
                strtmp = strtmp.ToLower().Replace("/inc/update.aspx", "");*/
                /*
                #region 将网址字符串转换为链接
                strtmp = strtmp.Replace("&amp;", "&");
                // p2p link
                strtmp = Regex.Replace(strtmp, @"^((ed2k|thunder|vagaa):\/\/[\[\]\|A-Za-z0-9\.\/=\?%\-&_~`@':+!]+)", "<a target=\"_blank\" href=\"$1\">$1</a>", RegexOptions.IgnoreCase);
                strtmp = Regex.Replace(strtmp, @"((ed2k|thunder|vagaa):\/\/[\[\]\|A-Za-z0-9\.\/=\?%\-&_~`@':+!]+)$", "<a target=\"_blank\" href=\"$1\">$1</a>", RegexOptions.IgnoreCase);
                strtmp = Regex.Replace(strtmp, @"[^>=\]""]((ed2k|thunder|vagaa):\/\/[\[\]\|A-Za-z0-9\.\/=\?%\-&_~`@':+!]+)", "<a target=\"_blank\" href=\"$1\">$1</a>", RegexOptions.IgnoreCase);


                //只在客户端端判断
                strtmp = Regex.Replace(strtmp, @"([^>=\]" + "\"" + @"'\/]|^)((((https?|ftp):\/\/)|www\.)([\w\-]+\.)*[\w\-\u4e00-\u9fa5]+\.([\.a-zA-Z0-9]+|\u4E2D\u56FD|\u7F51\u7EDC|\u516C\u53F8)((\?|\/|:)+[\w\.\/=\?%\-&~`@':+!]*)+\.(jpg|gif|png|bmp|jpeg))", "$1<img src=\"$2\" border=\"0\" />", RegexOptions.IgnoreCase);
                strtmp = Regex.Replace(strtmp, @"([^>=\]" + "\"" + @"'\/]|^)((((https?|ftp|gopher|news|telnet|rtsp|mms|callto|bctp|ed2k):\/\/)|www\.)([\w\-]+\.)*[\w\-\u4e00-\u9fa5]+\.([\.a-zA-Z0-9]+|\u4E2D\u56FD|\u7F51\u7EDC|\u516C\u53F8)((\?|\/|:)+[\w\.\/=\?%\-&~`@':+!#]*)*)", "$1<a target=\"_blank\" href=\"$2\">$2</a>", RegexOptions.IgnoreCase);
                strtmp = Regex.Replace(strtmp, @"([^\w>=\]:" + "\"" + @"'\.]|^)(([\-\.\w]+@[\.\-\w]+(\.\w+)+))", "$1<a href=\"mailto:$2\">$2</a>", RegexOptions.IgnoreCase);
                strtmp = Regex.Replace(strtmp, @"^((http||https|ftp|rtsp|mms|gopher|mailto|telnet|news|callto):\/\/[A-Za-z0-9\.\/=\?%\-&_~`@':+!]+)", "<a target=\"_blank\" href=\"$1\">$1</a>", RegexOptions.IgnoreCase);
                strtmp = Regex.Replace(strtmp, @"((http||https|ftp|rtsp|mms|gopher|mailto|telnet|news|callto):\/\/[A-Za-z0-9\.\/=\?%\-&_~`@':+!]+)$", "<a target=\"_blank\" href=\"$1\">$1</a>", RegexOptions.IgnoreCase);
                strtmp = Regex.Replace(strtmp, @"[^>=\]""]((http|https|ftp|rtsp|mms|gopher|mailto|telnet|news|callto):\/\/[A-Za-z0-9\.\/=\?%\-&_~`@':+!]+)", "<a target=\"_blank\" href=\"$1\">$1</a>", RegexOptions.IgnoreCase);


                #endregion
                 * */
                //适合于论坛贴子
                // strtmp = EncodeHtml(strtmp);
                //strtmp = strtmp.Replace(Convert.ToString((char)39), "''").Replace("'", "''");
                strtmp = strtmp.Replace("'", "''");
                strtmp = Regex.Replace(strtmp, @"#include", @"", RegexOptions.IgnoreCase);
                strtmp = eregiReplace("<(script)(.[^>]*)>|<(/script)>", "[$1$3]", strtmp, true);
                strtmp = eregiReplace("<iframe(.[^>]*)>(.[^<]*)|<iframe(.[^>]*)>(.[^<]*)(</iframe>)", "[框架页面]$2[/框架页面]", strtmp, true);
                /*strtmp = eregiReplace(@"onload=(.[^\s|^>]*)", @" ", strtmp, true);
                //超过580的宽都替为580
                strtmp = eregiReplace(@"width=(.{0,1})(5[7-9]\d|[6-9]\d{2}|\d{4,})(.{0,1})", @"width=580", strtmp, true);
                strtmp = eregiReplace(@"width:([\s]{0,1})(5[7-9]\d|[6-9]\d{2}|\d{4,})([\s]{0,1})px", @"width:580px", strtmp, true);
                strtmp = eregiReplace(@"onclick=(.[^\s|^>]*)", @" ", strtmp, true);
                strtmp = eregiReplace(@"<img(.*)height=(.[^\s|^>]*)", "<img$1", strtmp, true);*/
                //strtmp = eregiReplace("<img (.[^>]*)>", "<img onclick=\"ImgOpen(this);\" onload=\"ImgSizeAuto(this);\" $1 />", strtmp, true);
                //替换原字符串中的过敏字符
                //if (sconfig.text_replace) strtmp = BanWordFilter(strtmp);

            }
            return strtmp;
        }

        #region 判断来源页是否是合法的地址
        /// <summary>
        /// 判断来源页是否是合法的地址
        /// </summary>
        /// <param name="url">限定URL地址表</param>
        /// <param name="buffer">true为不缓存，false缓存</param>
        /// <returns>返回值为true时来源页合法，false时为不合法来源页</returns>
        /// <code>
        /// sharecode Limit=new sharecode();
        /// Hashtable url=new Hashtable();
        /// url.Add("1","www.5lin.com");
        /// url.Add("2","group.5lin.com");
        /// bool ok=Limit.domainAstrict(url,true);
        /// if(!ok) {
        ///     Response.Clear();
        ///     Response.Write("非法侵入！");
        ///     Response.End();
        /// }
        /// </code>
        public static bool domainAstrict(Hashtable url, bool buffer)
        {
            //判断来源
            try
            {
                string reurl = System.Web.HttpContext.Current.Request.UrlReferrer.DnsSafeHost;
                return domainAstrict(url, reurl, buffer);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断来源页是否是合法的地址
        /// </summary>
        /// <param name="url">限定URL地址表</param>
        /// <param name="buffer">true为不缓存，false缓存</param>
        /// <returns>返回值为true时来源页合法，false时为不合法来源页</returns>
        /// <code>
        /// sharecode Limit=new sharecode();
        /// Hashtable url=new Hashtable();
        /// url.Add("1","www.5lin.com");
        /// url.Add("2","group.5lin.com");
        /// bool ok=Limit.domainAstrict(url,true);
        /// if(!ok) {
        ///     Response.Clear();
        ///     Response.Write("非法侵入！");
        ///     Response.End();
        /// }
        /// </code>
        public static bool domainAstrict(Hashtable url, string astrurl, bool buffer)
        {
            try
            {
                IDictionaryEnumerator em = url.GetEnumerator();
                //判断来源
                string reurl = astrurl;
                /*bool isAstrict = false;
                while (em.MoveNext())
                {
                    if (reurl.IndexOf(em.Value.ToString()).ToString() != "-1")
                    {
                        isAstrict = true;
                    }
                }
                if (!isAstrict)
                {
                    return false;
                }
                else*/
                if (buffer)
                {
                    //不缓存页面
                    System.Web.HttpContext.Current.Response.Buffer = false;
                    System.Web.HttpContext.Current.Response.Expires = -1;
                    System.Web.HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 取得域名的后缀格式化域名
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns>返回域名后缀</returns>
        public static string domainSuffix(string domain)
        {
            string lastSuffix = domain.Substring(domain.LastIndexOf(".") + 1, domain.Length - domain.LastIndexOf(".") - 1);
            if (Regex.IsMatch(lastSuffix, "[a-zA-Z0-9]{1,20}"))
                return lastSuffix;
            else
                return "ch";
        }

        /// <summary>
        /// 取得域名的后缀格式化域名
        /// </summary>
        /// <param name="domain">域名</param>
        /// <returns>返回域名后缀</returns>
        public static string domainDotSuffix(string domain)
        {
            string lastSuffix = domain.Substring(domain.LastIndexOf(".") + 1, domain.Length - domain.LastIndexOf(".") - 1);
            if (Regex.IsMatch(lastSuffix, "[a-zA-Z0-9]{1,20}"))
                return "dot" + lastSuffix;
            else
                return "dotch";
        }

        /// <summary>
        /// 转换为简体中文
        /// </summary>
        public static string ToSChinese(string str)
        {
            return Strings.StrConv(str, VbStrConv.SimplifiedChinese, 0);
        }

        /// <summary>
        /// 转换为繁体中文
        /// </summary>
        public static string ToTChinese(string str)
        {
            return Strings.StrConv(str, VbStrConv.TraditionalChinese, 0);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (strContent.IndexOf(strSplit) < 0)
            {
                string[] tmp = { strContent };
                return tmp;
            }
            return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int p_3)
        {
            string[] result = new string[p_3];

            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < p_3; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 取得文件名的名称无扩展名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>返回文件名的名称无后缀</returns>
        public static string getFileName(string filename)
        {
            int start = filename.LastIndexOf("/") + 1;
            if (start < 0) start = 0;
            int end = filename.LastIndexOf(".");
            string lastSuffix = filename.Substring(start, (filename.Length - (start)) - (filename.Length - (end)));
            return lastSuffix;
        }

        #region 生成随机数 参数1生成数据的位数，参数2（1 为数字字母的混合，2数字，3字母，默认为数字字母的混合）
        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="codeCount">要生成的随机数位数</param>
        /// <returns></returns>
        public static string getRandom(int codeCount)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,m,n,l,o,p,q,r,s,t,u,v,w,x,y,z";
            string[] allCharArray = allChar.Split(',');
            int num = allCharArray.Length;
            string randomCode = "";
            int temp = -1;

            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                else
                    rand = new Random(i * ((int)DateTime.Now.Ticks));
                int t = rand.Next(num);
                if (temp == t)
                {
                    return getRandom(codeCount);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }

        /// <summary>
        /// 生成随机数
        /// </summary>
        /// <param name="codeCount">要生成的随机数位数</param>
        /// <param name="type">参数（1 为数字字母的混合，2数字，3字母，默认为数字字母的混合）</param>
        /// <returns></returns>
        public static string getRandom(int codeCount, int type)
        {
            string allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,m,n,l,o,p,q,r,s,t,u,v,w,x,y,z";
            switch (type)
            {
                case 1: { allChar = "0,1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,m,n,l,o,p,q,r,s,t,u,v,w,x,y,z"; break; }
                case 2: { allChar = "0,1,2,3,4,5,6,7,8,9"; break; }
                case 3: { allChar = "a,b,c,d,e,f,g,h,i,j,k,m,n,l,o,p,q,r,s,t,u,v,w,x,y,z"; break; }
            }
            string[] allCharArray = allChar.Split(',');
            int num = allCharArray.Length;
            string randomCode = "";
            int temp = -1;

            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                else
                    rand = new Random(i * ((int)DateTime.Now.Ticks));
                int t = rand.Next(num);
                if (temp == t)
                {
                    return getRandom(codeCount, type);
                }
                temp = t;
                randomCode += allCharArray[t];
            }
            return randomCode;
        }
        #endregion

        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// md5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string md5(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
        }

        /// <summary>
        /// 16位md5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string shortmd5(string str)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToLower().Substring(8, 16);
        }

        /// <summary>
        /// 将UTF-8格式字符转成GB2312
        /// </summary>
        /// <param name="strKeyValue"></param>
        /// <returns></returns>
        public static string getUtfChangeGB2312(string strKeyValue)
        {
            //return Encoding.GetEncoding("utf-8").GetString(Encoding.GetEncoding("GB2312").GetBytes(strKeyValue));

            Encoding en = Encoding.GetEncoding("gb2312");
            byte[] bytestr = en.GetBytes(strKeyValue + "　");
            Encoding cn = Encoding.GetEncoding("utf-8");
            return cn.GetString(bytestr, 0, bytestr.Length);

        }

        #region 信息提示框
        /// <summary>
        /// 信息提示框
        /// </summary>
        /// <param name="Message">提示信息</param>
        /// <param name="EnableBackHistory">是否返回上一页</param>
        public static void outMsgBox(string Message, bool EnableBackHistory)
        {
            string tmpback = "";
            if (EnableBackHistory)
            {

                tmpback = "window.history.back(1);";


                System.Web.HttpContext.Current.Response.Write("<script language=javascript>window.alert('" + Message + "');" + tmpback + "</script>");
                System.Web.HttpContext.Current.Response.End();
            }
            else
                System.Web.HttpContext.Current.Response.Write("<script language=javascript>window.alert('" + Message + "');</script>");
        }

        public static void outMsgBox(string Message, string path, bool EnableBackHistory)
        {
            string tmpback = "";
            string tmppath = "";
            if (EnableBackHistory)
            {
                if (path != "")
                {
                    tmppath = path;
                    tmppath = tmppath.Replace("window", "").Replace("javascript:", "");
                    if (path != tmppath)
                        tmpback = path;
                    else
                        tmpback = "window.location.href='" + path + "';";
                }
                else
                {
                    tmpback = "window.history.back(1);";
                }

                System.Web.HttpContext.Current.Response.Write("<script language=javascript>window.alert('" + Message + "');" + tmpback + "</script>");
                System.Web.HttpContext.Current.Response.End();
            }
            else
                System.Web.HttpContext.Current.Response.Write("<script language=javascript>window.alert('" + Message + "');" + tmpback + "</script>");
        }

        /// <summary>
        /// 弹出选项卡对话框
        /// </summary>
        /// <param name="Message">信息</param>
        /// <param name="trues">条件为真时</param>
        /// <param name="falses">条件为假</param>
        public static void outMsgBox(string Message, string trues, string falses)
        {
            System.Web.HttpContext.Current.Response.Write("<script language=javascript>returnoutMsgBox(); function returnoutMsgBox(){if(confirm('" + Message + "')){" + trues + "}" + falses + "}</script>");
        }

        /// <summary>
        /// 输入SCRIPT脚本
        /// </summary>
        /// <param name="str"></param>
        public static void outScript(string str)
        {
            System.Web.HttpContext.Current.Response.Write("<script language=javascript>" + str + ";</script>");
        }
        #endregion


        /// <summary>
        /// 读取节点的值
        /// </summary>
        public static XmlNodeList ad_info(string filename)
        {
            XmlNodeList node = null;
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(filename);

                XmlNodeList topM = xmldoc.DocumentElement.ChildNodes;
                foreach (XmlElement element in topM)
                {
                    if (element.Name == "scp")
                    {
                        node = element.ChildNodes;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return node;
            /*读取
             StreamReader reader1 = new StreamReader(Server.MapPath(@"/temp/oootue2.xml"), Encoding.GetEncoding("gb2312"));
            XmlDocument document1 = new XmlDocument();
            document1.LoadXml(reader1.ReadToEnd());
            XmlNodeList node1 = document1["scp"].ChildNodes;
            foreach (XmlNode element1 in node1)
            {
                XmlNodeList node2 = element1.ChildNodes;
                Response.Write("-");
                foreach (XmlElement element4 in node2)
                {
                    Response.Write(element4.Name + "," + element4.InnerText + "|<br>");
                }
                
            }
                
             */
        }
        /// <summary>
        /// 根据出生年月日返回年龄
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static int getAge(int year, int month, int day)
        {
            //DateTime agetime = new DateTime(year, month, day);
            DateTime thistime = DateTime.Now;
            int thisyear = thistime.Year - year;
            if (thistime.Month >= month)
            {
                if (thistime.Day > day)
                {
                    thisyear++;
                }
            }
            return thisyear;
        }

        #region 根据月日，返回所在星座
        /// <summary>
        /// 根据月日，返回所在星座
        /// </summary>
        /// <param name="month">月</param>
        /// <param name="day">日</param>
        /// <returns>星座</returns>
        public static string getConstellation(int month, int day)
        {
            string cons = "";
            switch (month)
            {
                case 3: //白羊座 3月21-4月20
                    {
                        if (day >= 21) cons = "白羊座";
                        else cons = "双鱼座";
                        break;
                    }
                case 4: //金牛座 4月21-5月21
                    {
                        if (day >= 21) cons = "金牛座";
                        else cons = "白羊座";
                        break;
                    }
                case 5: //双子座 5月22-6月21
                    {
                        if (day >= 22) cons = "双子座";
                        else cons = "金牛座";
                        break;
                    }
                case 6: //巨蟹座 6月22-7月22
                    {
                        if (day >= 22) cons = "巨蟹座";
                        else cons = "双子座";
                        break;
                    }
                case 7: //狮子座 7月23-8月23
                    {
                        if (day >= 23) cons = "狮子座";
                        else cons = "巨蟹座";
                        break;
                    }
                case 8: //处女座 8月24-9月23
                    {
                        if (day >= 24) cons = "处女座";
                        else cons = "狮子座";
                        break;
                    }
                case 9: //天秤座 9月24-10月23
                    {
                        if (day >= 24) cons = "天秤座";
                        else cons = "处女座";
                        break;
                    }
                case 10: //天蝎座 10月24-11月22
                    {
                        if (day >= 24) cons = "天蝎座";
                        else cons = "天秤座";
                        break;
                    }
                case 11: //射手座 11月23-12月21
                    {
                        if (day >= 23) cons = "射手座";
                        else cons = "天蝎座";
                        break;
                    }
                case 12: //魔羯座 12月22-1月20
                    {
                        if (day >= 22) cons = "魔羯座";
                        else cons = "射手座";
                        break;
                    }
                case 1: //水瓶座 1月21-2月19
                    {
                        if (day >= 21) cons = "水瓶座";
                        else cons = "魔羯座";
                        break;
                    }
                case 2: //双鱼座 2月20-3月20
                    {
                        if (day >= 20) cons = "双鱼座";
                        else cons = "水瓶座";
                        break;
                    }
            }
            return cons;
        }
        #endregion

        #region 根据年份，返回所在生肖
        /// <summary>
        /// 根据年份，返回所在生肖
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>生肖</returns>
        public static string getShenXiao(int year)
        {
            //余数：0、 1、 2、 3、 4、 5、 6、 7、 8、 9、10、11、

            //   猴、鸡、狗、猪、鼠、牛、虎、兔、龙、蛇、马、羊
            string shenxiao = "";
            int mod = 0;
            Math.DivRem(year, 12, out mod);
            switch (mod)
            {
                case 0: shenxiao = "猴年"; break;
                case 1: shenxiao = "鸡年"; break;
                case 2: shenxiao = "狗年"; break;
                case 3: shenxiao = "猪年"; break;
                case 4: shenxiao = "鼠年"; break;
                case 5: shenxiao = "牛年"; break;
                case 6: shenxiao = "虎年"; break;
                case 7: shenxiao = "兔年"; break;
                case 8: shenxiao = "龙年"; break;
                case 9: shenxiao = "蛇年"; break;
                case 10: shenxiao = "马年"; break;
                case 11: shenxiao = "羊年"; break;
            }
            return shenxiao;
        }
        #endregion

        #region 正则
        /// <summary>
        /// 用正则替换字符串
        /// </summary>
        /// <param name="eregi">正则</param>
        /// <param name="restring">要替换的字符</param>
        /// <param name="strsting">字符串</param>
        /// <returns></returns>
        public static string eregiReplace(string eregi, string newstring, string oldsting)
        {
            string s = Regex.Replace(oldsting, eregi, newstring);
            return s;
        }

        /// <summary>
        /// 用正则替换字符串
        /// </summary>
        /// <param name="eregi">正则</param>
        /// <param name="restring">要替换的字符</param>
        /// <param name="strsting">字符串</param>
        /// <param name="Case">是否不区分大小写</param>
        /// <returns></returns>
        public static string eregiReplace(string eregi, string newstring, string oldsting, bool Case)
        {
            string s = "";
            if (Case)
                s = Regex.Replace(oldsting, eregi, newstring, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            else
                s = Regex.Replace(oldsting, eregi, newstring);
            return s;
        }

        /// <summary>
        /// 用正则查询字符串内是否包含
        /// </summary>
        /// <param name="eregi">正则</param>
        /// <param name="restring">要替换的字符</param>
        /// <param name="strsting">字符串</param>
        /// <returns></returns>
        public static bool eregi(string eregi, string strsting)
        {
            return Regex.IsMatch(strsting, eregi, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
        #endregion

        #region 返回用户无头像用户默认照片
        /// <summary>
        /// 返回用户无头像用户默认照片
        /// </summary>
        /// <param name="UserSex">用户性别（男|女|未知）</param>
        /// <returns>返回用户默认头像照片路径</returns>
        public static string getNoPhoto(string UserSex)
        {
            string _return;
            switch (UserSex.ToString())
            {
                case "男": _return = "/public static /images/NoPhoto_boy.gif"; break;
                case "女": _return = "/public static /images/NoPhoto_girl.gif"; break;
                default: _return = "/public static /images/NoPhoto.gif"; break;
            }
            return _return;
        }
        #endregion

        #region 返回入库距离现在的时间
        /// <summary>
        /// 返回入库距离现在的时间
        /// </summary>
        /// <param name="time">数据库时间</param>
        /// <returns>距离现在的时间</returns>
        public static string getDiffDateTime(DateTime time)
        {
            return getDiffDateTime(time.ToString(), "MM-dd HH:mm");
        }
        /// <summary>
        /// 返回入库距离现在的时间
        /// </summary>
        /// <param name="time">数据库时间</param>
        /// <returns>距离现在的时间</returns>
        public static string getDiffDateTime(object time)
        {
            return getDiffDateTime(time, "MM-dd HH:mm");
        }
        /// <summary>
        /// 返回入库距离现在的时间
        /// </summary>
        /// <param name="time">数据库时间</param>
        /// <param name="def">默认格式:yyyy-MM-dd HH:mm:ss</param>
        /// <returns>距离现在的时间</returns>
        public static string getDiffDateTime(object time, string def)
        {
            if (!IsDateTime(time.ToString())) return "";
            DateTime dtime = DateTime.Now;
            DateTime stime = Convert.ToDateTime(time);
            TimeSpan ts = dtime.Subtract(stime);
            if (ts.TotalDays > 7 && ts.TotalDays <= 14) return "1周前";
            else if (ts.TotalDays <= 7 && ts.TotalDays >= 1) return "" + ts.Days.ToString() + "天前";
            else if (ts.TotalHours <= 24 && ts.TotalHours >= 1) return "" + ts.Hours.ToString() + "小时前";
            else if (ts.TotalMinutes <= 60 && ts.TotalMinutes > 2) return "" + ts.Minutes.ToString() + "分钟前";
            else if (ts.TotalMinutes < 2) return "刚刚发表";
            if (stime.Year == dtime.Year)
                return stime.ToString(def);
            else
                return stime.ToString("yyyy-MM-dd HH:mm");
        }
        #endregion

        #region 返回大小单位最小单位K，超过1024转为M，M超过1024转为G
        /// <summary>
        /// 返回大小单位最小单位K，超过1024转为M，M超过1024转为G
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static string getSizeUnits(double size)
        {
            if (size < 1024)
            {
                return size + "K";
            }
            size = Math.Round(size / 1024, 2);
            if (size < 1024)
            {
                return size + "M";
            }
            size = Math.Round(size / 1024, 2);
            return size + "G";
        }
        #endregion

        #region 返回文件类型所对应显示图片
        /// <summary>
        /// 返回文件类型所对应显示图片
        /// </summary>
        /// <param name="type">文件类型</param>
        /// <returns></returns>
        public static string getFileTypeImage(string type)
        {
            string img = "";
            switch (type.ToLower())
            {
                case "gif": img = "png.gif"; break;
                case "jpg": img = "image.gif"; break;
                case "pdf": img = "pdf.gif"; break;
                case "exe": img = "exe.gif"; break;
                case "doc": img = "doc.gif"; break;
                case "xls": img = "xls.gif"; break;
                case "htm": img = "html.gif"; break;
                case "html": img = "html.gif"; break;
                case "htmls": img = "html.gif"; break;
                case "txt": img = "text.gif"; break;
                case "rar": img = "zip.gif"; break;
                case "zip": img = "zip.gif"; break;
                default: img = "unknown.gif"; break;
            }
            return img;

        }
        #endregion

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /*
        /// <summary>
        /// 返回脏字过滤列表
        /// </summary>
        /// <returns></returns>
        public static string[,] GetBanWordList()
        {
            DNTCache cache = DNTCache.GetCacheService();
            string[,] str = cache.RetrieveObject("/Forum/BanWordList") as string[,];
            if (str != null)
            {
                return str;
            }
            sqlConnection sqlconn = new sqlConnection();
            string sql = "SELECT [find], [replacement] FROM words";
            using (DataTable dt = sqlconn.sqlReader(sql))
            {
                if (dt.Rows.Count > 0)
                {
                    str = new string[dt.Rows.Count, 2];
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        str[i, 0] = dt.Rows[i]["find"].ToString();
                        str[i, 1] = dt.Rows[i]["replacement"].ToString();
                    }
                }
            }
            cache.AddObject("/Forum/BanWordList", str);
            return str;
        }

        /// <summary>
        /// 替换原始字符串中的脏字词语
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <returns>替换后的结果</returns>
        public static string BanWordFilter(string text)
        {
            StringBuilder sb = new StringBuilder(text);
            string[,] str = this.GetBanWordList();
            if (str != null)
            {
                int count = str.Length / 2;
                for (int i = 0; i < count; i++)
                {
                    sb.Replace(str[i, 0], str[i, 1]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 判断字符串是否包含脏字词语
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <returns>如果包含则返回true, 否则反悔false</returns>
        public static bool InBanWordArray(string text)
        {
            string[,] str = this.GetBanWordList();
            if (str != null)
            {
                int count = str.Length / 2;
                for (int i = 0; i < count; i++)
                {
                    if (text.ToUpper().IndexOf(str[i, 0].ToUpper()) > -1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 脏字词语标注
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <returns>替换后的结果</returns>
        public static string BanWordMark(string text)
        {
            string style = "color:#ff0000;font-weight:bold;";
            return BanWordMark(text, style);
        }
        /// <summary>
        /// 脏字词语标注
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <param name="style">标注样式</param>
        /// <returns>替换后的结果</returns>
        public static string BanWordMark(string text, string style)
        {
            StringBuilder sb = new StringBuilder(text);
            string[,] str = this.GetBanWordList();
            if (str != null)
            {
                int count = str.Length / 2;
                for (int i = 0; i < count; i++)
                {
                    sb.Replace(str[i, 0], "<font style='" + style + "'>" + str[i, 0] + "</font>");
                }
            }
            return sb.ToString();
        }
        */
        /// <summary>
        /// 确认当前时间是否在指定的时间列表内
        /// </summary>
        /// <param name="timelist">一个包含多个时间段的列表(格式为hh:mm-hh:mm)</param>
        /// <param name="vtime">输出参数：符合条件的第一个是时间段</param>
        /// <returns>时间段存在则返回true,否则返回false</returns>
        public static bool BetweenTime(string timelist, out string vtime)
        {
            if (!timelist.Equals(""))
            {
                string[] enabledvisittime = SplitString(timelist, "|");

                if (enabledvisittime.Length > 0)
                {
                    string starttime = "";
                    int s = 0;
                    string endtime = "";
                    int e = 0;
                    foreach (string visittime in enabledvisittime)
                    {
                        if (Regex.IsMatch(visittime, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])-(([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9]))$"))
                        {
                            starttime = visittime.Substring(0, visittime.IndexOf("-"));
                            s = StrDateDiffMinutes(starttime, 0);

                            endtime = CutString(visittime, visittime.IndexOf("-") + 1, visittime.Length - (visittime.IndexOf("-") + 1));
                            e = StrDateDiffMinutes(endtime, 0);

                            if ((s > 0 && e < 0) || (s < 0 && e < 0 && (e > s)) || (s > 0 && e > 0 && (e > s)))
                            {
                                vtime = visittime;
                                return true;
                            }
                        }
                    }
                }
            }
            vtime = "";
            return false;
        }

        /// <summary>
        /// 确认当前时间是否在指定的时间列表内
        /// </summary>
        /// <param name="timelist">一个包含多个时间段的列表(格式为hh:mm-hh:mm)</param>
        /// <returns>时间段存在则返回true,否则返回false</returns>
        public static bool BetweenTime(string timelist)
        {
            string visittime = "";
            return BetweenTime(timelist, out visittime);
        }

        /// <summary>
        /// 判断文本内容中是否含有附件参数，true为包含附件
        /// </summary>
        /// <param name="text">文本内容</param>
        /// <returns></returns>
        public static bool inFiles(string text)
        {
            string re = @"(\.jpg|\.gif|\.png|\.bmp|\.jpeg|\.swf)+?";
            MatchCollection mat = Regex.Matches(text, re, RegexOptions.IgnoreCase);
            if (mat.Count > 0) return true;
            return false;
        }

        /// <summary>
        /// 返回显示IP的隐藏IP内容,隐藏部分用*表示ShowIP("192.168.1.10",3)显示为192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="partnum">显示段数</param>
        /// <returns></returns>
        public static string ShowIP(string ip, int partnum)
        {
            string[] userip = SplitString(ip, @".", 4);
            int ipIndex = 0;
            StringBuilder showip = new StringBuilder();
            for (ipIndex = 0; ipIndex < partnum; ipIndex++)
            {
                if (showip.Length > 0) showip.Append(@".");
                showip.Append(userip[ipIndex]);
            }

            while (ipIndex < 4)
            {
                if (showip.Length > 0) showip.Append(@".");
                showip.Append(@"*");
                ipIndex++;
            }

            return showip.ToString();
        }
        /// <summary>
        /// 返回显示字段的隐藏内容,隐藏部分用*表示ShowStr("张三",1)显示为张*
        /// </summary>
        /// <param name="str"></param>
        /// <param name="partnum">显示字数</param>
        /// <returns></returns>
        public static string ShowStr(string str, int partnum)
        {

            return ShowStr(str, partnum, 0);
        }
        /// <summary>
        /// 返回显示字段的隐藏内容,隐藏部分用*表示ShowStr("张三",1)显示为张*
        /// </summary>
        /// <param name="str"></param>
        /// <param name="partnum">显示字数</param>
        /// <param name="len">替换长度</param>
        /// <returns></returns>
        public static string ShowStr(string str, int partnum, int len)
        {
            return ShowStr(str, partnum, len, "*");
        }
        /// <summary>
        /// 返回显示字段的隐藏内容,隐藏部分用*表示ShowStr("张三",1)显示为张*
        /// </summary>
        /// <param name="str"></param>
        /// <param name="partnum">显示字数</param>
        /// <param name="len">替换长度</param>
        /// <param name="fh">默认*显示</param>
        /// <returns></returns>
        public static string ShowStr(string str, int partnum, int len, string fh)
        {
            if (str.Length <= partnum) return str;
            if (len == 0) len = str.Length - partnum;

            string showstr = str.Substring(0, partnum);

            str = str.Substring(partnum);
            if (str.Length > len) str = str.Substring(len);
            else str = "";

            var s = showstr + CreateMark(len, fh) + str;
            return s;

        }
        /// <summary>
        /// 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] iparray)
        {
            string[] userip = SplitString(ip, @".");
            for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
            {
                string[] tmpip = SplitString(iparray[ipIndex], @".");
                int r = 0;
                for (int i = 0; i < tmpip.Length; i++)
                {
                    if (tmpip[i] == "*")
                    {
                        return true;
                    }

                    if (userip.Length > i)
                    {
                        if (tmpip[i] == userip[i])
                        {
                            r++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                if (r == 4)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获得Assembly版本号
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyVersion()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(myAssembly.Location);
            return string.Format("{0}.{1}.{2}", myFileVersion.FileMajorPart, myFileVersion.FileMinorPart, myFileVersion.FileBuildPart);
        }

        /// <summary>
        /// 获得Assembly产品名称
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyProductName()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(myAssembly.Location);
            return myFileVersion.ProductName;
        }

        /// <summary>
        /// 获得Assembly产品版权
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyCopyright()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            FileVersionInfo myFileVersion = FileVersionInfo.GetVersionInfo(myAssembly.Location);
            return myFileVersion.LegalCopyright;
        }
        /// <summary>
        /// 建立文件夹
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool CreateDir(string name)
        {
            return MakeSureDirectoryPathExists(name);
        }

        /// <summary>
        /// 生成指定数量的html空格符号
        /// </summary>
        public static string Spaces(int nSpaces)
        {
            return CreateMark(nSpaces, " &nbsp;&nbsp;");
        }
        /// <summary>
        /// 生成指定数量的标记符号
        /// </summary>
        public static string CreateMark(int nSpaces, string markstr)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < nSpaces; i++)
            {
                sb.Append(markstr);
            }
            return sb.ToString();
        }
        /// <summary>
        /// 替换html字符
        /// </summary>
        public static string EncodeHtml(string strHtml)
        {
            if (strHtml != "")
            {
                strHtml = strHtml.Replace(",", "&def");
                strHtml = strHtml.Replace("'", "&dot");
                strHtml = strHtml.Replace(";", "&dec");
                return strHtml;
            }
            return "";
        }

        /// <summary>
        /// 为脚本替换特殊字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceStrToScript(string str)
        {
            str = str.Replace("\\", "\\\\");
            str = str.Replace("'", "\\'");
            str = str.Replace("\b", "\\b");
            str = str.Replace("\t", "\\t");
            str = str.Replace("\n", "\\n");
            str = str.Replace("\n", "\\n");
            str = str.Replace("\f", "\\f");
            str = str.Replace("\r", "\\r");
            return str.Replace("\"", "\\\"");
        }
        /// <summary>
        /// 为替换字符串特殊脚本
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ReplaceScriptToStr(string str)
        {
            str = str.Replace("\"", "&quot;");
            str = str.Replace("\'", "'");
            str = str.Replace("\\\\", "\\");
            return str;
        }
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>创建是否成功</returns>
        [DllImport("dbgHelp", SetLastError = true)]
        private static extern bool MakeSureDirectoryPathExists(string name);

        /// <summary>
        /// 返回相差的秒数
        /// </summary>
        /// <param name="Time"></param>
        /// <param name="Sec"></param>
        /// <returns></returns>
        public static int StrDateDiffSeconds(string Time, int Sec)
        {
            TimeSpan ts = DateTime.Now - DateTime.Parse(Time).AddSeconds(Sec);
            if (ts.TotalSeconds > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalSeconds < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalSeconds;
        }
        /// <summary>
        /// 返回距离结束时间相差的天数小时分钟数
        /// </summary>
        /// <param name="datetime">结束时间</param>
        /// <returns></returns>
        public static string StrDateDiffDaysTimesMinutes(string datetime)
        {
            string diff = string.Empty;
            DateTime now = new DateTime();
            now = DateTime.Now;
            DateTime endTime = datetime == "" ? DateTime.Now : Convert.ToDateTime(datetime);
            TimeSpan ts = new TimeSpan(endTime.Ticks - now.Ticks);
            if (Convert.ToDateTime(datetime).Ticks <= now.Ticks)
            {
                diff = "已结束";//string.Format("<b>{0}</b>天<b>{1}</b>小时<b>{2}</b>分钟");
            }
            else
            {
                diff = string.Format("<b>{0}</b>天<b>{1}</b>小时<b>{2}</b>分钟", ts.Days.ToString(), ts.Hours.ToString(), ts.Minutes.ToString());
            }
            return diff;
        }

        /// <summary>
        /// 返回相差的分钟数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="minutes"></param>
        /// <returns></returns>
        public static int StrDateDiffMinutes(string time, int minutes)
        {
            if (time == "" || time == null)
                return 1;
            TimeSpan ts = DateTime.Now - DateTime.Parse(time).AddMinutes(minutes);
            if (ts.TotalMinutes > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalMinutes < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalMinutes;
        }

        /// <summary>
        /// 返回相差的小时数
        /// </summary>
        /// <param name="time"></param>
        /// <param name="hours"></param>
        /// <returns></returns>
        public static int StrDateDiffHours(string time, int hours)
        {
            if (time == "" || time == null)
                return 1;
            TimeSpan ts = DateTime.Now - DateTime.Parse(time).AddHours(hours);
            if (ts.TotalHours > int.MaxValue)
            {
                return int.MaxValue;
            }
            else if (ts.TotalHours < int.MinValue)
            {
                return int.MinValue;
            }
            return (int)ts.TotalHours;
        }

        #region Cookies处理方式
        /// <summary>
        /// 写5LIN域下cookie值
        /// </summary>
        /// <param name="strName">项</param>
        /// <param name="strValue">值</param>
        public static void Write5LinCookie(string strName, string strValue)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["azsz%5FCookie"];
            if (cookie == null)
            {
                cookie = new HttpCookie("azsz%5FCookie");
                cookie.Values[strName] = UrlEncode(strValue);
            }
            else
            {
                cookie.Values[strName] = UrlEncode(strValue);
                if (HttpContext.Current.Request.Cookies["azsz%5FCookie"]["expires"] != null)
                {
                    int expires = StrToInt(HttpContext.Current.Request.Cookies["azsz%5FCookie"]["expires"].ToString(), 0);
                    if (expires > 0)
                    {
                        cookie.Expires = DateTime.Now.AddMinutes(StrToInt(HttpContext.Current.Request.Cookies["azsz%5FCookie"]["expires"].ToString(), 0));
                    }
                }
            }

            string cookieDomain = "5lin.com";
            if (cookieDomain != string.Empty && HttpContext.Current.Request.Url.Host.IndexOf(cookieDomain) > -1 && IsValidDomain(HttpContext.Current.Request.Url.Host))
                cookie.Domain = cookieDomain;
            HttpContext.Current.Response.AppendCookie(cookie);

        }


        /// <summary>
        /// 写5LIN域下cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="intValue">值</param>
        public static void Write5LinCookie(string strName, int intValue)
        {
            Write5LinCookie(strName, intValue.ToString());
        }


        /// <summary>
        /// 获得5LIN域下cookie值
        /// </summary>
        /// <param name="strName">项</param>
        /// <returns>值</returns>
        public static string Get5LinCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies["azsz%5FCookie"] != null && HttpContext.Current.Request.Cookies["azsz%5FCookie"][strName] != null)
            {
                return UrlDecode(HttpContext.Current.Request.Cookies["azsz%5FCookie"][strName].ToString());
            }

            return "";
        }

        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        public static void WriteCookie(string strName, string strValue)
        {
            string cookieDomain = "5lin.com";//webConfig.Cookies_domain
            WriteCookie(strName, strValue, 0, cookieDomain);
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="expires">过期时间分钟</param>
        public static void WriteCookie(string strName, string strValue, int expires)
        {
            string cookieDomain = "5lin.com";//webConfig.Cookies_domain
            WriteCookie(strName, strValue, expires, cookieDomain);
        }
        /// <summary>
        /// 写cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <param name="strValue">值</param>
        /// <param name="expires">过期时间分钟</param>
        /// <param name="cookieDomain">写入域</param>
        public static void WriteCookie(string strName, string strValue, int expires, string cookieDomain)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[strName];
            if (cookie == null)
            {
                cookie = new HttpCookie(strName);
            }
            cookie.Value = strValue;
            if (expires > 0) cookie.Expires = DateTime.Now.AddMinutes(expires);
            if (cookieDomain != string.Empty && HttpContext.Current.Request.Url.Host.IndexOf(cookieDomain) > -1 && IsValidDomain(HttpContext.Current.Request.Url.Host))
                cookie.Domain = cookieDomain;
            HttpContext.Current.Response.AppendCookie(cookie);
        }

        /// <summary>
        /// 读cookie值
        /// </summary>
        /// <param name="strName">名称</param>
        /// <returns>cookie值</returns>
        public static string GetCookie(string strName)
        {
            if (HttpContext.Current.Request.Cookies != null && HttpContext.Current.Request.Cookies[strName] != null)
            {
                return HttpContext.Current.Request.Cookies[strName].Value.ToString();
            }

            return "";
        }
        #endregion


        /// <summary>
        /// 是否为有效域
        /// </summary>
        /// <param name="host">域名</param>
        /// <returns></returns>
        public static bool IsValidDomain(string host)
        {
            Regex r = new Regex(@"^\d+$");
            if (host.IndexOf(".") == -1)
            {
                return false;
            }
            return r.IsMatch(host.Replace(".", string.Empty)) ? false : true;
        }

        /// <summary>
        /// 返回 HTML 字符串的编码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>编码结果</returns>
        public static string HtmlEncode(string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// 返回 HTML 字符串的解码结果
        /// </summary>
        /// <param name="str">字符串</param>
        /// <returns>解码结果</returns>
        public static string HtmlDecode(string str)
        {
            return HttpUtility.HtmlDecode(str);
        }
        /// <summary>
        /// 返回标准日期格式string
        /// </summary>
        public static string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// 返回指定日期格式
        /// </summary>
        public static string GetDate(string datetimestr, string replacestr)
        {
            if (datetimestr == null)
            {
                return replacestr;
            }

            if (datetimestr.Equals(""))
            {
                return replacestr;
            }

            try
            {
                datetimestr = Convert.ToDateTime(datetimestr).ToString("yyyy-MM-dd").Replace("1900-01-01", replacestr);
            }
            catch
            {
                return replacestr;
            }
            return datetimestr;

        }


        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetTime()
        {
            return DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回相对于当前时间的相对天数
        /// </summary>
        public static string GetDateTime(int relativeday)
        {
            return DateTime.Now.AddDays(relativeday).ToString("yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 返回标准时间格式string
        /// </summary>
        public static string GetDateTimeF()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fffffff");
        }

        /// <summary>
        /// 返回标准时间 
        /// </summary>
        public static string GetStandardDateTime(string fDateTime, string formatStr)
        {
            if (!IsDateTime(fDateTime)) return "";
            DateTime s = Convert.ToDateTime(fDateTime);
            return s.ToString(formatStr);
        }

        /// <summary>
        /// 返回标准时间 yyyy-MM-dd HH:mm:ss
        /// </summary>
        public static string GetStandardDateTime(string fDateTime)
        {
            return GetStandardDateTime(fDateTime, "yyyy-MM-dd HH:mm:ss");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsTime(string timeval)
        {
            return Regex.IsMatch(timeval, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }

        /// <summary>
        /// 判断字符串是否是yyyy-MM-dd字符串
        /// </summary>
        /// <param name="str">待判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsDateString(string str)
        {
            return Regex.IsMatch(str, @"(\d{4})-(\d{1,2})-(\d{1,2})");
        }

        public static string GetRealIP()
        {
            string ip = IPHelper.GetIP();
            return ip;
        }

        /// <summary>
        /// 清除HTML代码
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string ClearHtml(string strHtml)
        {
            if (strHtml != "")
            {
                Regex r = null;
                Match m = null;

                r = new Regex(@"<\/?[^>]*>", RegexOptions.IgnoreCase);
                for (m = r.Match(strHtml); m.Success; m = m.NextMatch())
                {
                    strHtml = strHtml.Replace(m.Groups[0].ToString(), "");
                }
            }
            return strHtml;
        }
        /// <summary>
        /// 移除Html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            string regexstr = @"<[^>]*>";
            return Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }

        /// <summary>
        /// 将用户组Title中的font标签去掉
        /// </summary>
        /// <param name="title">用户组Title</param>
        /// <returns></returns>
        public static string RemoveFontTag(string title)
        {
            Match m = RegexFont.Match(title);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            return title;
        }

        /// <summary>
        /// 进行指定的替换(脏字过滤)
        /// </summary>
        public static string StrFilter(string str, string bantext)
        {
            string text1 = "";
            string text2 = "";
            string[] textArray1 = SplitString(bantext, "\r\n");
            for (int num1 = 0; num1 < textArray1.Length; num1++)
            {
                text1 = textArray1[num1].Substring(0, textArray1[num1].IndexOf("="));
                text2 = textArray1[num1].Substring(textArray1[num1].IndexOf("=") + 1);
                str = str.Replace(text1, text2);
            }
            return str;
        }
        /// <summary>
        /// 取消Hmtl代码(2008/07/14)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DelHtml(string str)
        {
            str = Regex.Replace(str, @"\<(img)[^>]*>|<\/(img)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<(table|tbody|tr|td|th|)[^>]*>|<\/(table|tbody|tr|td|th|)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<(div|blockquote|fieldset|legend)[^>]*>|<\/(div|blockquote|fieldset|legend)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<(font|i|u|h[1-9]|s)[^>]*>|<\/(font|i|u|h[1-9]|s)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<(style|strong)[^>]*>|<\/(style|strong)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<a[^>]*>|<\/a>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<(meta|iframe|frame|span|tbody|layer)[^>]*>|<\/(iframe|frame|meta|span|tbody|layer)>", "", RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"\<br[^>]*", "", RegexOptions.IgnoreCase);
            return str;
        }

        /// <summary>
        /// 获取WAN IP Address
        /// </summary>
        /// <returns></returns>
        public static string GetWanIpAddress()
        {
            Uri uri = new Uri("http://www.ip138.com/ip2city.asp");
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            req.Method = "GET";

            using (HttpWebResponse res = (HttpWebResponse)(req.GetResponse()))
            {
                using (StreamReader rs = new StreamReader(res.GetResponseStream(), Encoding.GetEncoding("GB2312")))
                {
                    string s = rs.ReadToEnd();

                    rs.Close();
                    req.Abort();
                    res.Close();

                    s = s.Replace("您的IP地址是：[", "").Replace("]", "");
                    if (IsCorrenctIP(s))
                    {
                        return s;
                    }
                    else
                    {
                        return "127.0.0.1";
                    }
                }
            }
        }

        /// <summary>
        /// 正则 IP Address
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static bool IsCorrenctIP(string ip)
        {
            string pattrn = @"(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])";
            if (Regex.IsMatch(ip, pattrn))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 随机数
        /// </summary>
        /// <returns></returns>
        public static string MakeRandom(int length)
        {
            char[] s = new char[] { 
                '0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 
                'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                '~', '!', '@', '#', '$', '%', '^', '*', '(', ')', '-', '+', '=','{', '}'
            };
            string num = "";
            Random r = new Random();
            for (int i = 0; i < length; i++)
            {
                num += s[r.Next(0, s.Length)].ToString();
            }
            return num;
        }

        /// <summary>
        /// 获取状态展示方式
        /// </summary>
        /// <param name="_state"></param>
        /// <returns></returns>
        public static string getState(object _state)
        {
            return getState(_state, "", "");
        }
        /// <summary>
        /// 获取状态展示方式
        /// </summary>
        /// <param name="_state"></param>
        /// <returns></returns>
        public static string getState(object _state, string success, string error)
        {
            bool _s = false;
            if (_state.GetType().Name == "Boolean")
            {
                if ((bool)_state) _s = true;
            }
            else
            {
                if (StrToInt(_state, 0) == 1) _s = true;
            }
            if (_s) return String.Format("<img src=\"/images/yes.gif\" alt=\"{0}\" title=\"{0}\" />", success);
            else return String.Format("<img src=\"/images/no.gif\" alt=\"\" title=\"{0}\" />", error);
        }
        /// <summary>
        /// 获取是否可用
        /// </summary>
        /// <param name="_state"></param>
        /// <returns></returns>
        public static string getDisable(object _disable)
        {
            return getState(_disable, "", "");
        }
        /// <summary>
        /// 获取是否可用
        /// </summary>
        /// <param name="_disable"></param>
        /// <returns></returns>
        public static string getDisable(object _disable, string success, string error)
        {
            bool _s = false;
            if (_disable.GetType().Name == "Boolean")
            {
                if ((bool)_disable) _s = true;
            }
            else
            {
                if (StrToInt(_disable, 0) == 1) _s = true;
            }
            if (!_s) return String.Format("<img src=\"/images/yes.gif\" alt=\"{0}\" title=\"{0}\" />", error);
            else return String.Format("<img src=\"/images/no.gif\" alt=\"\" title=\"{0}\" />", success);
        }

        // <summary>
        /// 删除数组中的重复项
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static string RemoveDup(string values)
        {
            return RemoveDup(values, ",");
        }
        /// <summary>
        /// 删除数组中的重复项
        /// </summary>
        /// <param name="values">The values.</param>
        /// <param name="split">分隔符</param>
        /// <returns></returns>
        public static string RemoveDup(string values, string split)
        {
            string[] _values = RemoveDup(SplitString(values, split));
            return string.Join(split, _values);
        }
        /// <summary>
        /// 删除数组中的重复项
        /// </summary>
        /// <param name="values">The values.</param>
        /// <returns></returns>
        public static string[] RemoveDup(string[] values)
        {
            List<string> list = new List<string>();
            foreach (string s in values)
            {
                if (s == "") continue;
                if (list.IndexOf(s) == -1)
                    list.Add(s);
            }
            return list.ToArray();
        }
        /// <summary>
        /// 过滤SQL语句,防止注入
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns>0 - 没有注入, 1 - 有注入 </returns>
        public static int filterSql(string sSql)
        {
            int srcLen, decLen = 0;
            sSql = sSql.ToLower().Trim();
            srcLen = sSql.Length;
            sSql = sSql.Replace("exec", "");
            sSql = sSql.Replace("delete", "");
            sSql = sSql.Replace("master", "");
            sSql = sSql.Replace("truncate", "");
            sSql = sSql.Replace("declare", "");
            sSql = sSql.Replace("create", "");
            sSql = sSql.Replace("xp_", "no");
            decLen = sSql.Length;
            if (srcLen == decLen) return 0; else return 1;
        }

        /// <summary>
        /// string型转换为Long型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的Long类型结果</returns>
        public static long StrToLong(object strValue)
        {
            return StrToLong(strValue, 0);
        }
        /// <summary>
        /// string型转换为Long型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static long StrToLong(object strValue, long defValue)
        {
            if ((strValue == null) || (strValue.ToString() == string.Empty))
            {
                return defValue;
            }

            string val = strValue.ToString();
            long intValue = 0;
            if (long.TryParse(val, out intValue))
            {
                return intValue;
            }
            return defValue;
        }
        /// <summary>
        /// 输入Float格式数字，将其转换为货币表达方式
        /// </summary>
        /// <param name="ftype">货币表达类型：0=带￥的货币表达方式；1=不带￥的货币表达方式；其它=带￥的货币表达方式</param>
        /// <param name="fmoney">传入的int数字</param>
        /// <returns>返回转换的货币表达形式</returns>
        public static string Rmoney(int ftype, double fmoney)
        {
            string _rmoney;
            try
            {
                switch (ftype)
                {
                    case 0:
                        _rmoney = string.Format("￥{0:N2}", fmoney);
                        break;
                    case 1:
                        _rmoney = string.Format("{0:N2}", fmoney);
                        break;
                    default:
                        _rmoney = string.Format("￥{0:N2}", fmoney);
                        break;
                }
            }
            catch
            {
                _rmoney = "";
            }
            return _rmoney;
        }

        /// <summary>
        /// 将小数点后面的无效的0去掉
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        public static Decimal TrimDecimal(Decimal temp)
        {
            string strTemp = temp.ToString();
            string[] strs = strTemp.Split('.');
            if (strs.Length > 1)
            {
                string str = strs[1];
                if (str.Contains("00"))
                    return Decimal.Parse(strs[0]);
                if (strTemp.Contains(".0"))
                {
                    return temp;
                }
                if (str.Contains("0"))
                {
                    string ss = strTemp.Substring(0, strTemp.Length - 1);
                    return Decimal.Parse(ss);
                }
            }
            return temp;
        }

        /// <summary>
        /// 获取属性key value 对
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static IDictionary<String, Object> GetPropertiesValueDict<T>(T t)
        {
            IDictionary<String, Object> dict = new Dictionary<String, Object>();
            if (t == null)
            {
                return dict;
            }
            PropertyInfo[] properties = t.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            if (properties.Length <= 0)
            {
                return dict;
            }
            foreach (PropertyInfo item in properties)
            {
                dict.Add(item.Name, item.GetValue(t, null));
            }
            return dict;
        }

        /// <summary>
        /// 获取表单数据转换为实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="form"></param>
        /// <returns></returns>
        public static T ToModel<T>(this System.Collections.Specialized.NameValueCollection form, params string[] notcontainerfields) where T : new()
        {
            T model = new T();
            Type type = typeof(T);
            PropertyInfo[] ps = type.GetProperties();
            foreach (PropertyInfo p in ps)
            {
                if (!form[p.Name].IsEmpty() && !p.Name.EqualsIgnoreCase(notcontainerfields))
                {
                    type.GetProperty(p.Name).SetValue(model, Convert.ChangeType(form[p.Name], p.PropertyType), null);
                }
            }
            return model;
        }
    
    }
}

