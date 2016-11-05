using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web;
using YSL.Common.Assert;
using YSL.Common.Resources;
using System.IO;
using System.Globalization;
using Microsoft.VisualBasic;
namespace YSL.Common.Extender
{
    /// <summary>
    /// 字符串拓展类
    /// </summary>
    public static class StringExtender
    {
        private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        const string RTemplate = "|{0}|";

        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UrlEncode(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return HttpUtility.UrlEncode(input);
        }
        /// <summary>
        /// URL解码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string UrlDecode(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            return HttpUtility.UrlDecode(input);
        }
        /// <summary>
        /// 拼接URL
        /// </summary>
        /// <param name="baseUrlPath"></param>
        /// <param name="additionalNode"></param>
        /// <returns></returns>
        public static string Link(this string baseUrlPath, object additionalNode)
        {
            if (additionalNode == null)
            {
                return baseUrlPath;
            }
            if (baseUrlPath == null)
            {
                return additionalNode.ToString();
            }
            return Link(baseUrlPath, additionalNode.ToString());
        }
        /// <summary>
        /// 拼接URL
        /// </summary>
        /// <param name="baseUrlPath"></param>
        /// <param name="additionalNode"></param>
        /// <returns></returns>
        public static string Link(this string baseUrlPath, string additionalNode)
        {
            if (baseUrlPath == null)
            {
                return additionalNode;
            }
            if (additionalNode == null)
            {
                return baseUrlPath;
            }
            if (baseUrlPath.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                if (additionalNode.StartsWith("/", StringComparison.OrdinalIgnoreCase))
                {
                    baseUrlPath = baseUrlPath.TrimEnd(new char[] { '/' });
                }
                return baseUrlPath + additionalNode;
            }
            if (additionalNode.StartsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                return baseUrlPath + additionalNode;
            }
            return baseUrlPath + "/" + additionalNode;
        }
        /// <summary>
        /// 拼接URL
        /// </summary>
        /// <param name="baseUrlPath"></param>
        /// <param name="additionalNodes"></param>
        /// <returns></returns>
        public static string Link(this string baseUrlPath, params object[] additionalNodes)
        {
            var temp = baseUrlPath;
            foreach (var item in additionalNodes)
            {
                temp = temp.Link(item);
            }
            return temp;
        }
        /// <summary>
        /// Base64编码
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(this string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        /// <summary>
        /// Base64解码
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Decode(this string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        /// <summary>
        /// 格式化输出
        /// </summary>
        /// <param name="format"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Format(this string format, params object[] values)
        {
            return string.Format(format, values);
        }

        public static string EnSure(this string input, string defaultValue)
        {
            return string.IsNullOrEmpty(input) ? defaultValue : input;
        }
        /// <summary>
        /// 加密字符串
        /// </summary>
        /// <param name="normalString">普通的字符串</param>
        /// <returns>加密后的二进制字符串</returns>
        /// <remarks>不可逆加密</remarks>
        public static string Encrypt(this string normalString)
        {
            AssertUtil.NotNullOrWhiteSpace(normalString, Constant.StringIsNotNull);
            UnicodeEncoding encoding = new UnicodeEncoding();
            HashAlgorithm algroithm = new MD5CryptoServiceProvider();
            byte[] buffer = algroithm.ComputeHash(encoding.GetBytes(normalString));
            return BitConverter.ToString(buffer);
        }

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(this string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result);  //返回长度为44字节的字符串
        }

        /// <summary>
        /// 格式化字符串
        /// </summary>
        /// <param name="template">字符串模板</param>
        /// <param name="args">参数列表</param>
        /// <returns>格式化后的字符串</returns>
        public static string F(this string template, params object[] args)
        {
            return string.Format(template, args);
        }


        /// <summary>
        /// 代替字符串
        /// </summary>
        /// <param name="template">模版</param>
        /// <param name="oldValue">模版字符串</param>
        /// <param name="newValue">实际数据</param>
        /// <returns></returns>
        public static string R(this string template, string oldValue, string newValue)
        {
            return template.Replace(RTemplate.F(oldValue), newValue);
        }

        /// <summary>
        /// 测试字符串是否为空或者空字符组成
        /// </summary>
        /// <param name="msg">字符串</param>
        /// <returns>为空返回true</returns>
        public static bool IsNullOrWhiteSpace(this string msg)
        {
            return msg == null || msg == string.Empty;
        }

        /// <summary>
        /// 判断字符串source是否为空
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsEmpty(this string source)
        {
            return source == null || source.Trim().Length < 1;
        }

        /// <summary>
        /// 测试字符串不是为空或者空字符组成
        /// </summary>
        /// <param name="msg">字符串</param>
        /// <returns>为空返回true</returns>
        public static bool IsNotNullOrWhiteSpace(this string msg)
        {
            return !(msg == null || msg == string.Empty);
        }


        public static string FormatNull(this string msg)
        {
            if (msg.IsNullOrWhiteSpace())
                return msg;
            else
                return msg.Trim();
        }

        /// <summary>
        /// 字符串略缩显示
        /// </summary>
        /// <param name="msg">原字符串</param>
        /// <param name="length">长度</param>
        /// <returns>略缩后的字符串</returns>
        public static string Abbreviation(this string msg, int length)
        {
            if (msg.Length > length)
                return msg.Substring(0, length) + "...";
            else
                return msg;
        }

        /// <summary>
        /// 重复字符串
        /// </summary>
        /// <param name="msg">原字符串</param>
        /// <param name="count">重复次数</param>
        /// <returns>重复字符串</returns>
        public static string Duplication(this string msg, int count)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                builder.Append(msg);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 判断是否包含子字符
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="tmp">包含的子字符串</param>
        /// <returns>是否</returns>
        public static bool ContainsIgnoreCase(this string str, string tmp)
        {
            return str.ToLower().Contains(tmp.ToLower());
        }

        /// <summary>
        /// 比较字符串（不区分大小写）
        /// </summary>
        /// <param name="str">字符串A</param>
        /// <param name="str2">字符串B</param>
        /// <returns>是否相等</returns>
        public static bool EqualsIgnoreCase(this string str, string str2)
        {
            return str.Equals(str2, StringComparison.CurrentCultureIgnoreCase);
        }



        /// <summary>
        /// 反转字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="split"></param>
        /// <returns></returns>
        public static string ReverseFormat(this string str, char split = ',')
        {
            var tmps = str.Split(split).Reverse().ToArray();
            return string.Join(split.ToString(), tmps);
        }

        /// <summary>
        /// 清除无效的xml字符信息
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string CleanInvalidXmlChars(this string txt)
        {
            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(txt, r, "", RegexOptions.Compiled);
        }

        /// <summary>
        /// 特殊字符文本替换为XML格式
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToXMLContent(this string s)
        {
            if (s == null) return "";
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }


        /// <summary>
        /// 获取字符串哈希值
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SHA1(this string text)
        {
            byte[] cleanbytes = Encoding.Default.GetBytes(text);
            byte[] hashbytes = System.Security.Cryptography.SHA1.Create().ComputeHash(cleanbytes);
            return BitConverter.ToString(hashbytes);
        }




        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串
        /// <param name="encryptKey">加密密钥,要求为8位
        /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>
        public static string EncryptDES(this string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return encryptString;
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同
        /// <returns>解密成功返回解密后的字符串，失败返源串</returns>
        public static string DecryptDES(this string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return decryptString;
            }
        }
        /// <summary>
        /// MD5加密 utf-8
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Md5(this string str)
        {
            string codeType = "utf-8";
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(System.Text.Encoding.GetEncoding(codeType).GetBytes(str));
            System.Text.StringBuilder sb = new System.Text.StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string MD5Encrypt(this string str, Encoding encode)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(encode.GetBytes(str));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            return sb.ToString();
        }

        /// <summary>
        /// MD5对文件流加密
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static string MD5Encrypt(this string str, Stream stream)
        {
            MD5 md5serv = MD5CryptoServiceProvider.Create();
            byte[] buffer = md5serv.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte var in buffer)
                sb.Append(var.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// MD5加密(返回16位加密串)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string MD5Encrypt16(this string input, Encoding encode)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string result = BitConverter.ToString(md5.ComputeHash(encode.GetBytes(input)), 4, 8);
            result = result.Replace("-", "");
            return result;
        }
        /// <summary>
        /// URL路径加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string UrlPathEncode(this string str)
        {
            return HttpUtility.UrlPathEncode(str);
        }

        /// <summary>
        /// 汉字转换为Unicode编码
        /// </summary>
        /// <param name="str">要编码的汉字字符串</param>
        /// <returns>Unicode编码的的字符串</returns>
        public static string ToUnicode(this string str)
        {
            byte[] bts = Encoding.Unicode.GetBytes(str);
            string r = "";
            for (int i = 0; i < bts.Length; i += 2)
            {
                r += "\\u" + bts[i + 1].ToString("x").PadLeft(2, '0') + bts[i].ToString("x").PadLeft(2, '0');
            }
            return r;
        }
        /// <summary>
        /// 将Unicode编码转换为汉字字符串
        /// </summary>
        /// <param name="str">Unicode编码字符串</param>
        /// <returns>汉字字符串</returns>
        public static string ToGB2312(this string str)
        {
            MatchEvaluator me = new MatchEvaluator(delegate(Match m)
            {
                string strVal = "";
                if (m.Success)
                {
                    byte[] bts = new byte[2];
                    bts[0] = (byte)int.Parse(m.Groups[2].Value, NumberStyles.HexNumber);
                    bts[1] = (byte)int.Parse(m.Groups[1].Value, NumberStyles.HexNumber);
                    strVal = Encoding.Unicode.GetString(bts);
                }
                return strVal;
            });
            return Regex.Replace(str, @"\\u([\w]{2})([\w]{2})", me, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 转换为简体中文
        /// </summary>
        public static string ToSChinese(this string str)
        {
            return Strings.StrConv(str, VbStrConv.SimplifiedChinese, 0);
        }

        /// <summary>
        /// 转换为繁体中文
        /// </summary>
        public static string ToTChinese(this string str)
        {
            return Strings.StrConv(str, VbStrConv.TraditionalChinese, 0);
        }
        /// <summary>
        /// 返回指定类型的货币符号 
        /// </summary>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static string ToSymbol(this string currency)
        {
            if (string.IsNullOrEmpty(currency)) return string.Empty;
            switch (currency.ToUpper())
            {
                case "HKD":
                    return "$";
                case "CNY":
                    return "￥";
                case "ECB":
                    return "€";
                case "JPY":
                    return "¥";
                default:
                    return "$";
            }
        }

        /// <summary>
        /// 删除HTML标签脚本
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string TrimHtml(string value)
        {
            return value == null ? null : value.ToTrimHtml();
        }
        public static string ToTrimHtml(this string htmlStr)
        {
            //删除JS CSS脚本
            htmlStr = htmlStr.Replace("\r\n", "");
            htmlStr = Regex.Replace(htmlStr, @"<script.*?</script>", "", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"<style.*?</style>", "", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"<.*?>", "", RegexOptions.IgnoreCase);
            //删除HTML
            htmlStr = Regex.Replace(htmlStr, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"-->", "", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"--%>", "", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"<%--.*", "", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"&(lt|#60);", "", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"&(gt|#62);", "", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"&(nbsp|#160);", "", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlStr = Regex.Replace(htmlStr, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlStr = htmlStr.Replace("<", "");
            htmlStr = htmlStr.Replace(">", "");
            htmlStr = htmlStr.Replace("\r\n", "");
            htmlStr = System.Web.HttpContext.Current.Server.HtmlEncode(htmlStr).Trim();
            return htmlStr;
        }
        /// <summary>
        /// 手机号码用替换
        /// </summary>
        /// <param name="phoneStr"></param>
        /// <returns></returns>
        public static string ToPhone(this string phoneStr)
        {
            Regex regex = new Regex("(\\d{3})(\\d{4})(\\d{4})", RegexOptions.None);
            return regex.Replace(phoneStr, "$1****$3");
        }
        /// <summary>
        /// 邮箱替换
        /// </summary>
        /// <param name="emailStr"></param>
        /// <returns></returns>
        public static string ToEmail(this string emailStr)
        {
            var pattern = @"^(?<header>\w).*?@";
            Regex regex = new Regex(pattern);
            var match = regex.Match(emailStr);
            if (match.Success)
            {
                var replaceValue = match.Groups["header"].Value + "****@";
                return Regex.Replace(emailStr, pattern, replaceValue);
            }
            return emailStr;
        }
        /// <summary>
        /// 银行卡替换
        /// </summary>
        /// <param name="bankCardNumber"></param>
        /// <returns></returns>
        public static string ToBankCardNumber(this string bankCardNumber)
        {
            if (bankCardNumber.Length > 6)
            {
                Regex regex = new Regex("(\\d{6})(\\d*)");
                return regex.Replace(bankCardNumber, "$1***");
            }
            else
            {
                return bankCardNumber;
            }
        }
        /// <summary>
        /// 姓名替换
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ToAccountNameOrCompanyName(string name)
        {
            if (name.Length > 2)
            {
                return name.Substring(2, name.Length - 1) + "***";
            }
            else
            {
                return name;
            }
        }
        /// <summary>
        /// object型转换为string型
        /// </summary>
        /// <param name="objValue">要转换的对象</param>
        /// <returns>转换后的string类型结果</returns>
        public static string ObjectToString(object objValue)
        {
            if (objValue == null || objValue == DBNull.Value) return string.Empty;
            return objValue.ToString();
        }

        /// <summary>
        /// 判断b数组中是否包含a字符串(不区分大小写)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string a, params string[] b)
        {
            if (b == null || b.Length == 0)
            {
                return false;
            }
            foreach (string s in b)
            {
                if (string.Equals(a, s, StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
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

        /// <summary>
        /// 返回与 Web 服务器上的指定虚拟路径相对应的物理文件路径
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetPhysicalPath(string path)
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            if (path.IsEmpty() || path.Length == 1)
            {
                return basePath;
            }
            if (path.IndexOf('~') == 0)
            {
                path = path.Substring(1);
            }
            path = path.Replace('/', '\\');
            if (path.IndexOf('\\') == 0)
            {
                return basePath.Substring(0, basePath.Length - 1) + path;
            }
            else
            {
                return basePath.Substring(0, basePath.Length) + path;
            }
        }
    }
}
