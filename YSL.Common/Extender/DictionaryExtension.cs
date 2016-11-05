using System;
using System.Collections.Generic;

namespace YSL.Common.Extender
{
    /// <summary>
    /// Dictionary 扩展
    /// </summary>
    public static class DictionaryExtension
    {
        public static void Save<TKey, TValue>(this Dictionary<TKey, TValue> src, TKey key, TValue value)
        {
            if (src.ContainsKey(key))
            {
                src[key] = value;
            }
            else
            {
                src.Add(key, value);
            }
        }
        public static void Match<TKey, TValue>(this Dictionary<TKey, TValue> src, TKey key, Action<TValue> matched, Func<TValue> notMatched)
        {
            if (src.ContainsKey(key))
            {
                if (matched != null)
                {
                    matched(src[key]);
                }
            }
            else
            {
                if (notMatched != null)
                {
                    src.Add(key, notMatched());
                }
            }
        }

        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> src, TKey key)
        {
            if (src.ContainsKey(key))
            {
                src.Remove(key);
                return true;
            }
            return false;
        }
        public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> src, TKey key, out TValue value)
        {
            if (src.ContainsKey(key))
            {
                value = src[key];
                src.Remove(key);
                return true;
            }
            value = default(TValue);
            return false;
        }
    }
}