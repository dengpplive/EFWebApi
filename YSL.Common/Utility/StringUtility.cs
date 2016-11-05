using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YSL.Common.Extender;
namespace YSL.Common.Utility
{
    /// <summary>
    /// 字符串工具类
    /// </summary>
    public class StringUtility
    {
        /// <summary>
        /// 将枚举类型转换为javascript的形式返回
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public string EnumTypeToJavasceipt(List<Type> list)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Type item in list)
            {
                string arrayName = item.Name;
                sb.AppendFormat("var {0}=[];", arrayName);
                foreach (var enumItem in Enum.GetNames(item))
                {
                    var obj = Enum.Parse(item, enumItem, false);
                    string descStr = enumItem;
                    var descModel = obj.GetType().GetField(enumItem).GetCustomAttributes(typeof(DescriptionAttribute), false).OfType<DescriptionAttribute>().FirstOrDefault();
                    if (descModel != null)
                        descStr = descModel.Description;
                    sb.AppendFormat("{0}[{1}]={{Value:{1},Text:'{3}',Description:'{2}'}};", arrayName, (int)obj, descStr, enumItem);
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// 截取字符串的长度(中英文)
        /// </summary>
        /// <param name="source">处理的字符串</param>
        /// <param name="length">截取长度</param>
        /// <returns></returns>
        public static string GetSub(string source, int length, string ellipsis = "...")
        {
            if (source == null) return string.Empty;
            int len = length * 2;
            //aequilateLength为中英文等宽长度,cutLength为要截取的字符串长度
            int aequilateLength = 0, cutLength = 0;
            Encoding encoding = Encoding.GetEncoding("gb2312");
            string cutStr = source.ToString();
            int strLength = cutStr.Length;
            byte[] bytes;
            for (int i = 0; i < strLength; i++)
            {
                bytes = encoding.GetBytes(cutStr.Substring(i, 1));
                if (bytes.Length == 2)//不是英文
                    aequilateLength += 2;
                else
                    aequilateLength++;
                if (aequilateLength <= len) cutLength += 1;
                if (aequilateLength > len)
                    return cutStr.Substring(0, cutLength) + ellipsis;
            }
            return cutStr;
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return false;
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        public static bool IsIPSect(string ip)
        {
            if (string.IsNullOrEmpty(ip)) return false;
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
        }
        /// <summary>
        /// 验证是否只含有汉字
        /// </summary>
        /// <param name="strln">输入的字符</param>
        /// <returns></returns>
        public static bool IsChinese(string strln)
        {
            if (string.IsNullOrEmpty(strln)) return false;
            return Regex.IsMatch(strln, @"^[\u4e00-\u9fa5]+$");
        }

        /// <summary>
        /// 邮政编码 6个数字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsPostCode(string source)
        {
            if (string.IsNullOrEmpty(source)) return false;
            return Regex.IsMatch(source, @"^\d{6}$", RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 验证手机号码格式
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns></returns>
        public static bool IsMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return false;
            string pattern = @"^(13|14|15|17|18|19)[0-9]\d{8}$";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            return Regex.IsMatch(mobile, pattern);
        }

        /// <summary>
        /// Verifies that a string is in valid e-mail format
        /// </summary>
        /// <param name="email">Email to verify</param>
        /// <returns>true if the string is a valid e-mail address and false if it's not</returns>
        public static bool IsEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;
            email = email.Trim();
            return Regex.IsMatch(email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");

        }
        /// <summary>
        /// 验证固定电话
        /// </summary>
        /// <param name="phone">固定电话号码</param>
        /// <returns></returns>
        public static bool IsPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone)) return false;
            string pattern = @"((\d{11})|(400\d{7})|(400-(\d{4}|\d{3})-(\d{3}|\d{4}))|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)";
            Regex regex = new Regex(pattern, RegexOptions.Compiled);
            return Regex.IsMatch(phone, pattern);
        }
        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(object expression)
        {
            if (expression != null)
            {
                return IsNumeric(expression.ToString());
            }
            return false;

        }

        /// <summary>
        /// 判断对象是否为Int32类型的数字
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsNumeric(string expression)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                string str = expression;
                int nLen = Int32.MaxValue.ToString().Length;
                if (str.Length > 0 && str.Length <= nLen && Regex.IsMatch(str, @"^[-]?[0-9]*[.]?[0-9]*$"))
                {
                    if ((str.Length < 10) || (str.Length == nLen && str[0] == '1') || (str.Length == 11 && str[0] == '-' && str[1] == '1'))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 是否为Double类型
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static bool IsDouble(object expression)
        {
            if (expression != null)
            {
                return Regex.IsMatch(expression.ToString(), @"^([-]|[0-9])[0-9]*(\.[0-9]+)?([Ee][\+-][0-9]+)?$");
            }
            return false;
        }
        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsURL(string strUrl)
        {
            if (string.IsNullOrEmpty(strUrl)) return false;
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }
        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }
        /// <summary>
        /// 检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            if (string.IsNullOrEmpty(str)) return false;
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|游客|^Guest");
        }

        /// <summary>
        /// 判断字符串格式是否为1,2,3,4,5,6的数字列表，是返回true,为空或格式不正确返回false  
        /// </summary>
        /// <param name="NumberList">要判断字符串</param>
        /// <returns></returns>
        public static bool IsInNumberList(string numberList)
        {
            if (numberList.IsNullOrWhiteSpace())
                return false;
            return Regex.IsMatch(numberList, "^[0-9]{1,9}([,]{1,1}[0-9]{1,9})*$");
        }
        /// <summary>
        /// 类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T ChangeType<T>(object obj)
        {
            if (obj == null)
            {
                return default(T);
            }
            if ((obj is string) && string.IsNullOrEmpty(Convert.ToString(obj)))
            {
                return default(T);
            }
            return (T)Convert.ChangeType(obj, typeof(T));
        }
        #region 验证图片文件
        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImgFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return false;
            fileName = fileName.Trim();
            if (fileName.EndsWith(".") || fileName.IndexOf(".") == -1)
                return false;
            string extName = fileName.Substring(fileName.LastIndexOf(".") + 1).ToLower();
            string[] tmpExtArray = new string[] { "jpg", "jpeg", "png", "bmp", "gif" };
            return InArray(extName, tmpExtArray);
        }
        /// <summary>
        /// 根据文件头判断上传的文件类型
        /// </summary>
        /// <param name="stream">文件类型</param>
        /// <returns>返回true或false</returns>
        public static bool CheckImageByStream(Stream stream)
        {
            bool isImage = false;
            try
            {
                System.Drawing.Image image = System.Drawing.Image.FromStream(stream);//严重更正过来
                isImage = true;
                image.Dispose();
                image = null;
            }
            catch (Exception ex)
            {
                isImage = false;
            }
            return isImage;
        }
        /// <summary>
        /// 根据二进制获取文件
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static bool IsImgFileByBytes(byte[] buffer)
        {

            /*文件扩展名说明
             * 208207 doc xls ppt wps
             * 8075 docx pptx xlsx zip
             * 5150 txt
             * 8297 rar
             * 7790 exe
             * 3780 pdf      
             * 
             * 4946/104116 txt
             * 
             * 7173        gif 
             * 255216      jpg
             * 13780       png
             * 6677        bmp
             * 239187      txt,aspx,asp,sql
             * 208207      xls.doc.ppt
             * 6063        xml
             * 6033        htm,html
             * 4742        js
             * 8075        xlsx,zip,pptx,mmap,zip
             * 8297        rar   
             * 01          accdb,mdb
             * 7790        exe,dll
             * 5666        psd 
             * 255254      rdp 
             * 10056       bt种子 
             * 64101       bat 
             * 4059        sgf    
             */
            if (buffer == null || buffer.Length < 2) return false;
            string extByte = buffer[0].ToString() + buffer[1];
            string[] tmpExtArray = new string[] { "7173", "255216", "13780", "6677" };
            return InArray(extByte, tmpExtArray);

        }
        /// <summary>
        /// 根据二进制获取文件
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static bool IsImgFileByMime(byte[] buffer)
        {
            string mime = GetMimeFromByBytes(buffer);
            string[] tmpMimeArray = new string[] { "image/pjpeg", "image/jpeg", "image/jpeg2000", "image/png", "image/x-png", "image/bmp", "image/gif" };
            return InArray(mime, tmpMimeArray);
        }
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static System.UInt32 FindMimeFromData(
            System.UInt32 pBC,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
            [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
            System.UInt32 cbSize,
            [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
            System.UInt32 dwMimeFlags,
            out System.UInt32 ppwzMimeOut,
            System.UInt32 dwReserverd
        );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static string GetMimeFromByBytes(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0) return string.Empty;
            try
            {
                System.UInt32 mimetype;
                FindMimeFromData(0, null, buffer, 256, null, 0, out mimetype, 0);
                System.IntPtr mimeTypePtr = new IntPtr(mimetype);
                string mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
                return mime;
            }
            catch (Exception ex)
            {
                return string.Empty; ;
            }
        }
        #endregion

        /// <summary>
        /// 检查是否含有非法字符
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <returns></returns>
        public static bool ChkBadChar(string str)
        {
            bool result = false;
            if (string.IsNullOrEmpty(str))
                return result;
            string strBadChar, tempChar;
            string[] arrBadChar;
            strBadChar = "@@,+,',--,%,^,&,?,(,),<,>,[,],{,},/,\\,;,:,\",\"\"";
            arrBadChar = SplitString(strBadChar, ",");
            tempChar = str;
            for (int i = 0; i < arrBadChar.Length; i++)
            {
                if (tempChar.IndexOf(arrBadChar[i]) >= 0)
                    result = true;
            }
            return result;
        }

        /// <summary>
        /// 移除Html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            return Regex.Replace(content, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
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
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!string.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                {
                    string[] tmp = { strContent };
                    return tmp;
                }
                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
            {
                return new string[0] { };
            }
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            string[] result = new string[count];

            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }


        /// <summary>
        /// to HexString
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static String Bytes2Hex(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder("");
            String tmp = null;
            for (int i = 0; i < bytes.Length; i++)
            {
                tmp = (bytes[i] & 0xFF).ToString("x2");
                if (tmp.Length == 1)
                {
                    builder.Append("0");
                }
                builder.Append(tmp);
            }
            return builder.ToString();
        }
        public static byte[] Hex2Byte(byte[] b)
        {
            if ((b.Length % 2) != 0)
            {
                throw new ArgumentException("长度不是偶数");
            }
            byte[] b2 = new byte[b.Length / 2];
            for (int n = 0; n < b.Length; n += 2)
            {
                String item = Encoding.UTF8.GetString(b, n, 2);
                b2[n / 2] = (byte)Convert.ToInt32(item, 16);
            }
            b = null;
            return b2;
        }

        /**
         * 将int 转换为 byte 数组
         *
         * @param i
         * @return
         */
        public static byte[] IntToByteArray(int i)
        {
            byte[] result = new byte[4];
            result[0] = (byte)((i >> 24) & 0xFF);
            result[1] = (byte)((i >> 16) & 0xFF);
            result[2] = (byte)((i >> 8) & 0xFF);
            result[3] = (byte)(i & 0xFF);
            return result;
        }

        /**
         * 将byte数组 转换为int
         *
         * @param b
         * @param offset 位游方式
         * @return
         */
        public static int ByteArrayToInt(byte[] b, int offset)
        {
            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                int shift = (4 - 1 - i) * 8;
                value += (b[i + offset] & 0x000000FF) << shift;//往高位游
            }
            return value;
        }
        /// <summary>
        /// 随机字母和数字的获取
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetRandomStr(int length)
        {
            string c = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int count = c.Length;
            string str = string.Empty;
            Random rd = new Random();
            for (int i = 0; i < length; i++)
            {
                int num = rd.Next(0, count);
                str += c[i];
            }
            return str;
        }
        /// <summary>
        /// 按位生随机码
        /// </summary>
        /// <param name="codeCount"></param>
        /// <returns></returns>
        public static string CreateRandomCode(int codeCount = 4)
        {
            int number;
            string randomCode = string.Empty;
            Random random = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                number = random.Next(100);
                switch (number % 3)
                {
                    case 0:
                        randomCode += ((char)('0' + (char)(number % 10))).ToString();
                        break;
                    case 1:
                        randomCode += ((char)('a' + (char)(number % 26))).ToString();
                        break;
                    case 2:
                        randomCode += ((char)('A' + (char)(number % 26))).ToString();
                        break;
                    default:
                        break;
                }
            }
            return randomCode;
        }
        ///// <summary>
        ///// 客户端是否来自微信浏览器
        ///// </summary>
        ///// <param name="contrller"></param>
        ///// <returns></returns>
        //public static bool IsWeChatBrowser(ControllerBase contrller)
        //{
        //    var result = false;
        //    if (contrller.ControllerContext.HttpContext.Request.UserAgent.ToLower().Contains("micromessenger"))
        //    {
        //        result = true;
        //    }
        //    else
        //    {
        //        result = false;
        //    }
        //    return result;
        //}
        ///// <summary>
        ///// 获取客户端类型
        ///// </summary>
        ///// <param name="contrller"></param>
        ///// <returns></returns>
        //public static ClientType GetClientType(ControllerBase contrller)
        //{
        //    ClientType type = ClientType.Other;
        //    if (contrller.ControllerContext.HttpContext.Request.UserAgent.ToLower().Contains("ios"))
        //    {
        //        type = ClientType.IOS;
        //    }
        //    else if (contrller.ControllerContext.HttpContext.Request.UserAgent.ToLower().Contains("android"))
        //    {
        //        type = ClientType.Android;
        //    }
        //    return type;
        //}

        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <returns></returns>
        public static string GenerateTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }
}
