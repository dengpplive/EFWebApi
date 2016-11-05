using System.Collections.Generic;
using System.Linq;
namespace YSL.Common.Extender
{
    public static class ArrayExtensions {
        /// <summary>
        /// 比较两个数组中的元素是否相同。
        /// </summary>
        /// <typeparam name="T">数组中的元素的类型。</typeparam>
        /// <param name="arrSrc">用于比较的第一个数组。</param>
        /// <param name="array">用于比较的第二个数组。</param>
        /// <returns>如果两个数组在相同索引位置的元素相等，返回 true；否则返回 false。</returns>
        public static bool Equals<T>(this T[] arrSrc, T[] array) {
            if (arrSrc == null || array == null || arrSrc.Length != array.Length) { return false; }
            return !arrSrc.Where((t, i) => !t.Equals(array[i])).Any();
        }

        /// <summary>
        /// 比较两个数组中的元素是否相同。
        /// </summary>
        /// <typeparam name="T">数组中的元素的类型。</typeparam>
        /// <param name="arrSrc">用于比较的第一个数组。</param>
        /// <param name="array">用于比较的第二个数组。</param>
        /// <param name="comparer">用于判断两个元素相等的比较器。</param>
        /// <returns>如果两个数组在相同索引位置的元素相等，返回 true；否则返回 false。</returns>
        public static bool Equals<T>(this T[] arrSrc, T[] array, IEqualityComparer<T> comparer) {
            if (comparer == null)
                return Equals(arrSrc, array);

            if (arrSrc == null || array == null || arrSrc.Length != array.Length) { return false; }
            return !arrSrc.Where((t, i) => !comparer.Equals(t, array[i])).Any();
        }

        /// <summary>
        /// 比较两个数组中的元素是否相同。
        /// </summary>
        /// <param name="arrSrc">用于比较的第一个数组。</param>
        /// <param name="array">用于比较的第二个数组。</param>
        /// <returns>如果两个数组在相同索引位置的元素相等，返回 true；否则返回 false。</returns>
        public static bool Equals(this object[] arrSrc, object[] array) {
            if (arrSrc == null || array == null || arrSrc.Length != array.Length) { return false; }
            return !arrSrc.Where((t, i) => !t.Equals(array[i])).Any();
        }

        /// <summary>
        /// 比较两个数组中的元素是否相同。
        /// </summary>
        /// <param name="arrSrc">用于比较的第一个数组。</param>
        /// <param name="array">用于比较的第二个数组。</param>
        /// <param name="comparer">用于判断两个元素相等的比较器。</param>
        /// <returns>如果两个数组在相同索引位置的元素相等，返回 true；否则返回 false。</returns>
        public static bool Equals(this object[] arrSrc, object[] array, IEqualityComparer<object> comparer) {
            if (comparer == null)
                return Equals(arrSrc, array);

            if (arrSrc == null || array == null || arrSrc.Length != array.Length) { return false; }
            return !arrSrc.Where((t, i) => !comparer.Equals(t, array[i])).Any();
        }

    }
}
