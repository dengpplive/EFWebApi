using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace YSL.Common.Extender
{
    public static class ExpandTypeConvert
    {
        public static int ToInt(this string str, int defaultValue = 0)
        {
            int.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static decimal ToDecimal(this string str, decimal defaultValue = 0)
        {
            decimal.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static short ToShort(this string str, short defaultValue = 0)
        {
            short.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static bool ToBool(this string str, bool defaultValue = false)
        {
            if (!string.IsNullOrEmpty(str))
            {
                string strTemp = str.ToLower();
                if (string.Compare(strTemp, "true", true) == 0 || strTemp == "1")
                    defaultValue = true;
                else if (string.Compare(strTemp, "false", true) == 0 || strTemp == "0")
                    defaultValue = false;
            }
            bool.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static bool ToBool(this object expression, bool defValue)
        {
            if (expression != null)
            {
                return ToBool(expression.ToString(), defValue);
            }
            return defValue;
        }
        public static byte ToByte(this string str, byte defaultValue = 0)
        {
            byte.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static byte ToByte(this object str, byte defaultValue = 0)
        {
            if (str != null && str != "")
                byte.TryParse(str.ToString(), out defaultValue);

            return defaultValue;
        }
        public static char ToChar(this string str, char defaultValue = '0')
        {
            char.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static float ToFloat(this string str, float defaultValue = 0)
        {
            float.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static long ToLong(this string str, long defaultValue = 0)
        {
            long.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static sbyte ToSbyte(this string str, sbyte defaultValue = 0)
        {
            sbyte.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static uint ToUint(this string str, uint defaultValue = 0)
        {
            uint.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static ulong ToUlong(this string str, ulong defaultValue = 0)
        {
            ulong.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static ushort ToUshort(this string str, ushort defaultValue = 0)
        {
            ushort.TryParse(str, out defaultValue);
            return defaultValue;
        }

        public static DateTime ToDateTime(this string str, DateTime defaultValue)
        {
            DateTime.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static Guid ToGuid(this string str, Guid defaultValue = default(Guid))
        {
            Guid.TryParse(str, out defaultValue);
            return defaultValue;
        }
        public static T ToEnum<T>(this string str, T defaultValue = default(T)) where T : struct
        {
            Enum.TryParse<T>(str, out defaultValue);
            return defaultValue;
        }

        public static T JsonToObject<T>(this string str, T defaultValue = default(T))
        {
            try
            {
                defaultValue = JsonConvert.DeserializeObject<T>(str);
            }
            catch { }
            return defaultValue;
        }
        public static string ToJson(this object obj, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            string r = string.Empty;
            try
            {
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
                timeFormat.DateTimeFormat = dateTimeFormat;
                r = JsonConvert.SerializeObject(obj, timeFormat);
            }
            catch { }
            return r;
        }


        public static object ToObject(this byte[] bytes)
        {
            MemoryStream streamMemory = new MemoryStream(bytes);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(streamMemory);
        }
        public static T ToObject<T>(this byte[] bytes) where T : class
        {
            MemoryStream streamMemory = new MemoryStream(bytes);
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(streamMemory) as T;
        }
        public static string ToBase64String(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        public static string ToStr(this byte[] bytes)
        {
            return System.Text.Encoding.Default.GetString(bytes);
        }
        public static int ToInt(this byte[] bytes)
        {
            return BitConverter.ToInt32(bytes, 0);
        }
        public static bool ToBoolean(this byte[] bytes)
        {
            return BitConverter.ToBoolean(bytes, 0);
        }
        public static char ToChar(this byte[] bytes)
        {
            return BitConverter.ToChar(bytes, 0);
        }
        public static double ToDouble(this byte[] bytes)
        {
            return BitConverter.ToDouble(bytes, 0);
        }
        public static short ToInt16(this byte[] bytes)
        {
            return BitConverter.ToInt16(bytes, 0);
        }
        public static long ToInt64(this byte[] bytes)
        {
            return BitConverter.ToInt64(bytes, 0);
        }
        public static float ToSingle(this byte[] bytes)
        {
            return BitConverter.ToSingle(bytes, 0);
        }
        public static ushort ToUInt16(this byte[] bytes)
        {
            return BitConverter.ToUInt16(bytes, 0);
        }
        public static uint ToUInt32(this byte[] bytes)
        {
            return BitConverter.ToUInt32(bytes, 0);
        }
        public static ulong ToUInt64(this byte[] bytes)
        {
            return BitConverter.ToUInt64(bytes, 0);
        }
        public static DateTime ToDateTime(this byte[] bytes, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            var str = bytes.ToStr();
            return str.ToDateTime(default(DateTime));
        }

        public static byte[] ToBase64Bytes(this string str)
        {
            return Convert.FromBase64String(str);
        }

        public static byte[] GetBytes(this string str)
        {
            return System.Text.Encoding.Default.GetBytes(str);
        }
        public static byte[] GetBytes(this int value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this bool value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this char value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this double value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this float value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this long value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this short value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this uint value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this ulong value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this ushort value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] GetBytes(this DateTime value, string dateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            return value.ToString(dateTimeFormat).GetBytes();
        }
        public static byte[] ToBytes<T>(this T obj) where T : class
        {
            MemoryStream streamMemory = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(streamMemory, obj);
            return streamMemory.GetBuffer();
        }
        public static string ToMd5(this string inStr)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(inStr, "MD5");
        }
        public static string ToMd5(this byte[] data)
        {
            return FormsAuthentication.HashPasswordForStoringInConfigFile(BitConverter.ToString(data), "MD5");
        }
        public static string ToMd5(this Stream stream)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            return BitConverter.ToString(provider.ComputeHash(stream)).Replace("-", "");
        }
        public static string FileToMd5(this string filepath)
        {
            using (FileStream stream = File.OpenRead(filepath))
            {
                return ToMd5(stream);
            }
        }
        /// <summary>
        /// 转换人民币大小金额
        /// </summary>
        /// <param name="num">金额</param>
        /// <returns>返回大写形式</returns>
        private static string ToChinaRMB(this decimal num)
        {
            string str1 = "零壹贰叁肆伍陆柒捌玖";            //0-9所对应的汉字
            string str2 = "万仟佰拾亿仟佰拾万仟佰拾元角分"; //数字位所对应的汉字
            string str3 = "";    //从原num值中取出的值
            string str4 = "";    //数字的字符串形式
            string str5 = "";  //人民币大写金额形式
            int i;    //循环变量
            int j;    //num的值乘以100的字符串长度
            string ch1 = "";    //数字的汉语读法
            string ch2 = "";    //数字位的汉字读法
            int nzero = 0;  //用来计算连续的零值是几个
            int temp;            //从原num值中取出的值

            num = Math.Round(Math.Abs(num), 2);    //将num取绝对值并四舍五入取2位小数
            str4 = ((long)(num * 100)).ToString();        //将num乘100并转换成字符串形式
            j = str4.Length;      //找出最高位
            if (j > 15) { return "溢出"; }
            str2 = str2.Substring(15 - j);   //取出对应位数的str2的值。如：200.55,j为5所以str2=佰拾元角分

            //循环取出每一位需要转换的值
            for (i = 0; i < j; i++)
            {
                str3 = str4.Substring(i, 1);          //取出需转换的某一位的值
                temp = Convert.ToInt32(str3);      //转换为数字
                if (i != (j - 3) && i != (j - 7) && i != (j - 11) && i != (j - 15))
                {
                    //当所取位数不为元、万、亿、万亿上的数字时
                    if (str3 == "0")
                    {
                        ch1 = "";
                        ch2 = "";
                        nzero = nzero + 1;
                    }
                    else
                    {
                        if (str3 != "0" && nzero != 0)
                        {
                            ch1 = "零" + str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                    }
                }
                else
                {
                    //该位是万亿，亿，万，元位等关键位
                    if (str3 != "0" && nzero != 0)
                    {
                        ch1 = "零" + str1.Substring(temp * 1, 1);
                        ch2 = str2.Substring(i, 1);
                        nzero = 0;
                    }
                    else
                    {
                        if (str3 != "0" && nzero == 0)
                        {
                            ch1 = str1.Substring(temp * 1, 1);
                            ch2 = str2.Substring(i, 1);
                            nzero = 0;
                        }
                        else
                        {
                            if (str3 == "0" && nzero >= 3)
                            {
                                ch1 = "";
                                ch2 = "";
                                nzero = nzero + 1;
                            }
                            else
                            {
                                if (j >= 11)
                                {
                                    ch1 = "";
                                    nzero = nzero + 1;
                                }
                                else
                                {
                                    ch1 = "";
                                    ch2 = str2.Substring(i, 1);
                                    nzero = nzero + 1;
                                }
                            }
                        }
                    }
                }
                if (i == (j - 11) || i == (j - 3))
                {
                    //如果该位是亿位或元位，则必须写上
                    ch2 = str2.Substring(i, 1);
                }
                str5 = str5 + ch1 + ch2;
                if (i == j - 1 && str3 == "0")
                {
                    //最后一位（分）为0时，加上“整”
                    str5 = str5 + '整';
                }
            }
            if (num == 0)
            {
                str5 = "零元整";
            }
            return str5;
        }

        #region IDataReader
        /// <summary>
        /// DataReader转泛型
        /// </summary>
        /// <typeparam name="T">传入的实体类</typeparam>
        /// <param name="objReader">DataReader对象</param>
        /// <returns></returns>
        public static List<T> ReaderToList<T>(this IDataReader objReader)
        {
            using (objReader)
            {
                List<T> list = new List<T>();

                //获取传入的数据类型
                Type modelType = typeof(T);

                //遍历DataReader对象
                while (objReader.Read())
                {
                    //使用与指定参数匹配最高的构造函数，来创建指定类型的实例
                    T model = Activator.CreateInstance<T>();
                    for (int i = 0; i < objReader.FieldCount; i++)
                    {
                        //判断字段值是否为空或不存在的值
                        if (!IsNullOrDBNull(objReader[i]))
                        {
                            //匹配字段名
                            PropertyInfo pi = modelType.GetProperty(objReader.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (pi != null)
                            {
                                //绑定实体对象中同名的字段  
                                pi.SetValue(model, CheckType(objReader[i], pi.PropertyType), null);
                            }
                        }
                    }
                    list.Add(model);
                }
                return list;
            }
        }

        /// <summary>
        /// 对可空类型进行判断转换(*要不然会报错)
        /// </summary>
        /// <param name="value">DataReader字段的值</param>
        /// <param name="conversionType">该字段的类型</param>
        /// <returns></returns>
        private static object CheckType(object value, Type conversionType)
        {
            if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;
                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }
            return Convert.ChangeType(value, conversionType);
        }

        /// <summary>
        /// 判断指定对象是否是有效值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool IsNullOrDBNull(object obj)
        {
            return (obj == null || (obj is DBNull)) ? true : false;
        }


        /// <summary>
        /// DataReader转模型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objReader"></param>
        /// <returns></returns>
        public static T ReaderToModel<T>(this IDataReader objReader)
        {
            using (objReader)
            {
                if (objReader.Read())
                {
                    Type modelType = typeof(T);
                    int count = objReader.FieldCount;
                    T model = Activator.CreateInstance<T>();
                    for (int i = 0; i < count; i++)
                    {
                        if (!IsNullOrDBNull(objReader[i]))
                        {
                            PropertyInfo pi = modelType.GetProperty(objReader.GetName(i), BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                            if (pi != null)
                            {
                                pi.SetValue(model, CheckType(objReader[i], pi.PropertyType), null);
                            }
                        }
                    }
                    return model;
                }
            }
            return default(T);
        }
        #endregion

        #region HttpRequestMessage 获取ip
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage =
            "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";

        public static string ToIpAddress(this HttpRequestMessage request)
        {
            // Web-hosting. Needs reference to System.Web.dll
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            // Self-hosting. Needs reference to System.ServiceModel.dll. 
            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            // Self-hosting using Owin. Needs reference to Microsoft.Owin.dll. 
            if (request.Properties.ContainsKey(OwinContext))
            {
                dynamic owinContext = request.Properties[OwinContext];
                if (owinContext != null)
                {
                    return owinContext.Request.RemoteIpAddress;
                }
            }

            return null;
        }
        #endregion

        public static T GetCustomAttribute<T>(this Type type) where T : Attribute
        {
            return type.GetCustomAttributes(typeof(T), true).FirstOrDefault() as T;
        }

        public static string ToMoneyString(this decimal d)
        {
            string ret = string.Format("${0:###,###,###.##}", d);
            if (ret == "$")
                return "$0.00";
            else
                return ret;
        }
        public static string ToMoneyString(this decimal d, string currencySymbol)
        {
            string symbol = currencySymbol.ToSymbol();
            string ret = string.Format("{0:###,###,###.##}", d);
            if (ret == "")
                return string.Concat(symbol, "0.00");
            else
                return string.Concat(symbol, ret);
        }

        /// <summary>
        /// 计算年龄         
        /// </summary>
        /// <param name="birthday">生日</param>
        /// <returns>年龄</returns>
        public static int ToAge(DateTime birthday)
        {
            int intAge = 0;

            int nowYear = DateTime.Now.Year;
            int nowMonth = DateTime.Now.Month + 1;
            int nowDay = DateTime.Now.Day;
            int birYear = birthday.Year;
            int birMonth = birthday.Month;
            int birDay = birthday.Day;

            intAge = nowYear - birYear - 1;
            if (birMonth <= nowMonth && birDay <= nowDay)
            {
                intAge++;
            }

            return intAge;
        }
        /// <summary>
        /// 1,2,4,8,16这类字符（余求和）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int ToOrSum(this string str)
        {
            if (str.IsEmpty())
            {
                return 0;
            }
            IList<int> list = str.Split(',').Select(p => p.ToInt()).ToList();
            int count = 0;
            foreach (int i in list)
            {
                count = count | i;
            }
            return count;
        }
    }
}
