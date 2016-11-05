using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Extender
{
    /// <summary>
    /// 枚举类的扩展类
    /// </summary>
    public static class EnumExtender
    {
        static Hashtable hs = new Hashtable();
        /// <summary>
        /// 将指定枚举类型的项以键值对的形式列举出来
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<KeyValuePair<T?, string>> GetItemList<T>() where T : struct
        {
            var result = hs[typeof(T?)] as List<KeyValuePair<T?, string>>;
            if (result == null)
            {
                var fs = typeof(T).GetFields().Select(p => new
                {
                    p,
                    att = p.GetCustomAttributes(false).FirstOrDefault(q => q is DescriptionAttribute) as DescriptionAttribute
                }).Where(p => p.att != null).ToList();


                result = new List<KeyValuePair<T?, string>>();

                foreach (var f in fs)
                {
                    T t = (T)Enum.Parse(typeof(T), f.p.GetValue(null).ToString());
                    result.Add(new KeyValuePair<T?, string>(t, f.att.Description));
                }
                hs[typeof(T)] = result;
            }
            return result;
        }
        /// <summary>
        /// 获取枚举的Description值
        /// </summary>
        /// <param name="source">枚举对象</param>
        /// <returns>枚举描述</returns>
        public static string ToDesc(this Enum source)
        {
            if (source == null) throw new ArgumentException("source");
            var type = source.GetType();
            if (Enum.IsDefined(type, source))
            {
                var field = type.GetField(Enum.GetName(type, source));
                if (field != null)
                {
                    if (Attribute.IsDefined(field, typeof(DescriptionAttribute)))
                    {
                        var descriptionAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                        if (descriptionAttribute != null)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }
            return source.ToString();

            //if (source == null) return string.Empty;
            //Type sourceType = source.GetType();
            //string sourceName = Enum.GetName(sourceType, source);
            //FieldInfo field = sourceType.GetField(sourceName);
            //object[] attributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            //foreach (object attribute in attributes)
            //{
            //    if (attribute is DescriptionAttribute)
            //    {
            //        var desc = attribute as DescriptionAttribute;
            //        if (desc == null) return "";
            //        else
            //            return desc.Description;
            //    }
            //}
            //return source.ToString();
        }      
    }
}
