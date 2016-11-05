using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
namespace ExcelACE
{
    /// <summary>
    /// some extensions
    /// </summary>
    public static class Extensions
    {
        private static readonly Dictionary<string, Delegate> Parsers = new Dictionary<string, Delegate>();

        private static Func<string, T> GetParser<T>()
        {
            var typeName = typeof(T).FullName;
            if (!Parsers.ContainsKey(typeName))
            {
                lock (Parsers)
                {
                    if (!Parsers.ContainsKey(typeName))
                    {
                        var parser = CreateParser<T>();
                        if (parser == null) { return null; }
                        Parsers.Add(typeName, parser);
                    }
                }
            }
            return (Func<string, T>)Parsers[typeName];
        }

        private static Func<string, T> CreateParser<T>()
        {
            var method = typeof(T).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            if (method != null)
            {
                var arg = Expression.Parameter(typeof(string), "str");
                var exp = Expression.Call(null, method, (Expression)arg);
                return Expression.Lambda<Func<string, T>>(exp, arg).Compile();
            }
            return null;
        }

        private static object CreateNullValue(object value)
        {
            if (value == null) { return null; }
            var type = value.GetType();
            var nullableType = typeof(Nullable<>);
            if (type.IsClass || type.IsGenericType && type.GetGenericTypeDefinition() == nullableType) { return value; }
            return Activator.CreateInstance(nullableType.MakeGenericType(type), value);
        }

        private static int[] GetSplittedIndex(int length, int count)
        {
            var idxArray = new int[count];
            for (var i = 0; i < count; i++)
            {
                var leftCount = count - i;
                var divResult = length / leftCount;
                idxArray[i] = length % leftCount == 0 ? divResult : divResult + 1;
                length -= idxArray[i];
            }
            return idxArray;
        }

        /// <summary>
        /// 将序列尽可能的平均分割成指定段。
        /// </summary>
        /// <param name="source">要分割的源序列。</param>
        /// <param name="count">要分割的段数。</param>
        /// <typeparam name="T">序列中元素类型。</typeparam>
        /// <returns>返回一个包含分割后的序列的序列。</returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int count)
        {
            var srcArray = source.ToArray();
            var idxArray = GetSplittedIndex(srcArray.Length, count);
            var start = 0;
            for (var i = 0; i < count; i++)
            {
                var arr = new T[idxArray[i]];
                Array.Copy(srcArray, start, arr, 0, arr.Length);
                start += arr.Length;
                yield return arr;
            }
        }

        /// <summary>
        /// 获取 XML 节点的属性值。
        /// </summary>
        /// <param name="node">要获取属性值的节点。</param>
        /// <param name="attrName">属性名称。</param>
        /// <returns>如果属性存在，返回该属性的值，否则返回 null。</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string GetAttribute(this XmlNode node, string attrName)
        {
            if (node == null) { throw new ArgumentNullException("node"); }
            if (string.IsNullOrWhiteSpace(attrName)) { throw new ArgumentNullException("attrName"); }
            var attrs = node.Attributes;
            if (attrs != null)
            {
                var attr = attrs[attrName];
                if (attr != null) { return attr.Value; }
            }
            return null;
        }

        /// <summary>
        /// 获取数据记录中指定名称的字段的值。
        /// </summary>
        /// <param name="record">要获取数据的数据记录。</param>
        /// <param name="fieldName">字段名称。</param>
        /// <typeparam name="T">字段值的类型。</typeparam>
        /// <returns>返回指定字段的值。</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public static T Get<T>(this IDataRecord record, string fieldName)
        {
            if (record == null) { throw new ArgumentNullException("record"); }
            if (string.IsNullOrWhiteSpace(fieldName)) { throw new ArgumentNullException("fieldName"); }
            var idx = record.GetOrdinal(fieldName);
            if (record.IsDBNull(idx)) { return default(T); }

            var resultType = typeof(T);
            if (resultType == typeof(string)) { return (T)(object)record[idx].ToString(); }

            var fieldType = record.GetFieldType(idx);
            if (resultType.IsAssignableFrom(fieldType)) { return (T)record[idx]; }

            if (resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Nullable<>)) { return (T)CreateNullValue(record[fieldName]); }
            var str = record[idx].ToString();
            if (string.IsNullOrEmpty(str)) { return default(T); }
            var parser = GetParser<T>();
            if (parser == null) { throw new InvalidCastException(string.Format("无法将类型 \"{0}\" 的值转换为 类型 \"{1}\"。", fieldType.FullName, typeof(T).FullName)); }
            return parser(str);
        }
    }
}
