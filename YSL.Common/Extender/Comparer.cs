using System;
using System.Collections.Generic;

namespace YSL.Common.Extender
{
    /// <summary>
    /// 比较器
    /// </summary>
    public static class Comparer
    {
        public static T Max<T>(params T[] value) where T : struct, IComparable<T>
        {
            if (value == null || value.Length == 0)
                throw new ArgumentNullException("value");

            var result = value[0];
            for (var i = 1; i < value.Length; i++)
            {
                var item = value[i];
                if (result.CompareTo(item) < 0)
                {
                    result = item;
                }
            }
            return result;
        }
        public static T Min<T>(params T[] value) where T : struct, IComparable<T>
        {
            if (value == null || value.Length == 0)
                throw new ArgumentNullException("value");

            var result = value[0];
            for (var i = 1; i < value.Length; i++)
            {
                var item = value[i];
                if (result.CompareTo(item) > 0)
                {
                    result = item;
                }
            }
            return result;
        }

        public static T? Max<T>(params T?[] value) where T : struct, IComparable<T>
        {
            if (value == null)
                return null;

            T? result = null;
            foreach (var item in value)
            {
                if (item == null)
                    continue;
                if (result == null)
                {
                    result = item;
                }
                else
                {
                    if (result.Value.CompareTo(item.Value) < 0)
                    {
                        result = item;
                    }
                }
            }
            return result;
        }
        public static T? Min<T>(params T?[] value) where T : struct, IComparable<T>
        {
            if (value == null)
                return null;

            T? result = null;
            foreach (var item in value)
            {
                if (item == null)
                    continue;
                if (result == null)
                {
                    result = item;
                }
                else
                {
                    if (result.Value.CompareTo(item.Value) > 0)
                    {
                        result = item;
                    }
                }
            }
            return result;
        }
    }
}