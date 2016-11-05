using System.Collections.Generic;
using System.Linq;

namespace YSL.Common.Extender
{
    /// <summary>
    /// 针对 HashSet 的扩展。
    /// </summary>
    public static class HashSetExtensions {
        /// <summary>
        /// 向 HashSet 中添加一组元素。
        /// </summary>
        /// <typeparam name="T">元素类型。</typeparam>
        /// <param name="set">要向其中添加元素的 HashSet。</param>
        /// <param name="range">包含要添加的元素的序列。</param>
        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> range) {
            if (set != null && range != null) {
                foreach (var item in range.Where(item => !set.Contains(item))) {
                    set.Add(item);
                }
            }
        }
    }
}