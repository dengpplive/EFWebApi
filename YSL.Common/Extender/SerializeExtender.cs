using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using YSL.Common.Assert;

namespace YSL.Common.Extender
{
    /// <summary>
    /// 序列化
    /// </summary>
    public static class SerializeExtender
    {
        #region 二进制
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">原始数据</param>
        /// <returns>二进制</returns>
        public static byte[] ToBinary(this object obj)
        {
            AssertUtil.IsNotNull(obj);
            BinaryFormatter f = new BinaryFormatter();
            using (MemoryStream inStream = new MemoryStream())
            {
                f.Serialize(inStream, obj);//对象序列化 
                inStream.Position = 0;
                return inStream.ToArray();
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="buffer">二进制</param>
        /// <returns>原始数据</returns>
        public static object FromBinary(this byte[] buffer)
        {
            AssertUtil.IsNotNull(buffer);
            BinaryFormatter f = new BinaryFormatter();
            using (MemoryStream inStream = new MemoryStream(buffer))
            {
                return f.Deserialize(inStream);
            }
        }
        #endregion

        #region XML
        /// <summary>
        /// 将对象序列化为XML内容
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="obj">对象</param>
        /// <returns>xml内容</returns>
        public static string Serialize<T>(this T obj)
        {
            string xml = "";
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                try
                {
                    ser.Serialize(ms, obj);
                }
                catch
                {
                }
                ms.Position = 0;
                using (StreamReader sr = new StreamReader(ms))
                {
                    xml = sr.ReadToEnd();
                    sr.Dispose();
                }
                ms.Dispose();
            }
            return xml;
        }
        /// <summary>
        /// 将XML内容反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="xml">xml内容</param>
        /// <returns>对象类型</returns>
        public static T Deserialize<T>(this string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(T));
                    return (T)ser.Deserialize(sr);
                }
            }
            catch
            {
                return default(T);
            }
        }


        #endregion

        #region JSON格式
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="obj">原始对象</param>
        /// <returns>JSON格式字符串</returns>
        public static string ToJSON(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// JSON字符串转换为对象
        /// </summary>
        /// <param name="json">JSON格式字符串</param>
        /// <returns>对象</returns>
        public static T FromJSON<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        #endregion

        /// <summary>
        /// 正则表达式验证
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="regularExpression">正则表达式</param>
        /// <returns>源字符串是否匹配正则表达式</returns>
        public static bool IsMatch(this string str, string pattern)
        {
            Regex reg = new Regex(pattern);
            return reg.IsMatch(str);
        }
    }
}
