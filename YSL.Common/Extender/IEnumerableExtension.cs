using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using YSL.Common.Utility;

namespace YSL.Common.Extender
{
    public static class IEnumerableExtension
    {
        public static string Join(this IEnumerable<string> source, string separator = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            separator = separator ?? string.Empty;
            if (source.Any())
            {
                //if (source.Count() > 10) {
                var build = new StringBuilder();
                var etor = source.GetEnumerator();
                var count = source.Count();
                var pos = 1;
                while (etor.MoveNext())
                {
                    build.Append(etor.Current);
                    if (pos < count)
                    {
                        build.Append(separator);
                    }
                    pos++;
                }
                return build.ToString();
                //}
                //return source.Aggregate((x, y) => x + separator + y);
            }
            return string.Empty;
        }
        public static string Join<TSource>(this IEnumerable<TSource> source, string separator, Func<TSource, string> map)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (map == null)
                throw new ArgumentNullException("map");

            return source.Any() ? Join(source.Select(map), separator) : string.Empty;
        }

        public static string Join(this string[] set, string op)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < set.Length; i++)
            {
                if (i > 0)
                {
                    sb.Append(op);
                }
                sb.Append(set[i]);
            }
            return sb.ToString();
        }

        [Obsolete]
        public static IEnumerable<string> Filter(this IEnumerable<string> source)
        {
            return source.Distinct();
        }

        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource> comparer)
        {
            var set = new Set<TSource>(comparer);
            return source.Where(set.Add);
        }

        /// <summary>
        /// 将序列转换为只读集合
        /// </summary>
        /// <typeparam name="TElement"> 元素类型 </typeparam>
        /// <param name="set"> 被扩展的序列 </param>
        /// <returns> </returns>
        public static ReadOnlyCollection<TElement> ToReadOnly<TElement>(this IEnumerable<TElement> set)
        {
            var roc = set as ReadOnlyCollection<TElement>;
            if (roc == null)
            {
                if (set == null)
                {
                    roc = EmptyReadOnlyCollection<TElement>.Empty;
                }
                else
                {
                    roc = new List<TElement>(set).AsReadOnly();
                }
            }
            return roc;
        }

        public static void ForeachWhile<TSource>(this IEnumerable<TSource> source, Action<TSource> action, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (action == null)
                throw new ArgumentNullException("action");

            if (predicate == null)
            {
                ForEach(source, action);
            }
            else
            {
                foreach (var item in source)
                {
                    if (!predicate(item))
                        return;
                    action(item);
                }
            }
        }
        /// <summary>
        /// 循环测试指定条件，并在满足时为序列中的元素执行指定操作，否则退出循环。
        /// </summary>
        /// <typeparam name="TSource">序列中的元素的类型。</typeparam>
        /// <param name="sequence">要为元素执行操作的序列。</param>
        /// <param name="action">要为元素执行的操作。</param>
        /// <param name="predicate">测试条件。</param>
        public static void ForeachWhile<TSource>(this IEnumerable<TSource> sequence, Action<TSource, int> action, Func<TSource, int, bool> predicate = null)
        {
            if (sequence == null)
                throw new ArgumentNullException("sequence");
            if (action != null)
            {
                if (predicate == null)
                {
                    sequence.For(action);
                }
                else
                {
                    var array = sequence.ToArray();
                    for (var i = 0; i < array.Length; i++)
                    {
                        if (predicate(array[i], i))
                        {
                            action(array[i], i);
                            continue;
                        }
                        break;
                    }
                }
            }
        }
        public static void ForeachWhen<TSource>(this IEnumerable<TSource> source, Action<TSource> action, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (action == null)
                throw new ArgumentNullException("action");
            if (predicate == null)
                throw new ArgumentNullException("predicate");

            foreach (var item in source)
            {
                if (predicate(item))
                {
                    action(item);
                }
            }
        }
        public static void ForeachExcept<TSource>(this IEnumerable<TSource> source, Action<TSource> action, Func<TSource, bool> predicate)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (action == null)
                throw new ArgumentNullException("action");
            if (predicate == null)
            {
                ForEach(source, action);
            }
            else
            {
                foreach (var item in source)
                {
                    if (!predicate(item))
                    {
                        action(item);
                    }
                }
            }
        }

        public static TSource MinElement<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector, IComparer<TValue> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (source.Any())
                return MinOrDefaultElement(source, selector, comparer);
            throw new InvalidOperationException("序列 source 中不包含任何元素");
        }
        public static TSource MinOrDefaultElement<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector, IComparer<TValue> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (selector == null)
                throw new ArgumentNullException("selector");

            var minElement = default(TSource);
            var minValue = default(TValue);
            comparer = comparer ?? Comparer<TValue>.Default;
            var hasValue = false;
            foreach (var item in source)
            {
                var curValue = selector(item);
                if (hasValue)
                {
                    if (comparer.Compare(curValue, minValue) < 0)
                    {
                        minValue = curValue;
                        minElement = item;
                    }
                }
                else
                {
                    minValue = curValue;
                    minElement = item;
                    hasValue = true;
                }
            }
            return minElement;
        }
        public static TSource MaxElement<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector, IComparer<TValue> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (source.Any())
                return MaxOrDefaultElement(source, selector, comparer);
            throw new InvalidOperationException("序列 source 中不包含任何元素");
        }
        public static TSource MaxOrDefaultElement<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector, IComparer<TValue> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (selector == null)
                throw new ArgumentNullException("selector");

            var maxElement = default(TSource);
            var maxValue = default(TValue);
            comparer = comparer ?? Comparer<TValue>.Default;
            var hasValue = false;
            foreach (var item in source)
            {
                var curValue = selector(item);
                if (hasValue)
                {
                    if (comparer.Compare(curValue, maxValue) > 0)
                    {
                        maxValue = curValue;
                        maxElement = item;
                    }
                }
                else
                {
                    maxValue = curValue;
                    maxElement = item;
                    hasValue = true;
                }
            }
            return maxElement;
        }

        public static TSource MinElement<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (source.Any())
                return MinOrDefaultElement(source, comparer);
            throw new InvalidOperationException("序列 source 中不包含任何元素");
        }
        public static TSource MinOrDefaultElement<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var minElement = default(TSource);
            comparer = comparer ?? Comparer<TSource>.Default;
            var hasValue = false;
            foreach (var item in source)
            {
                if (hasValue)
                {
                    if (comparer.Compare(item, minElement) < 0)
                    {
                        minElement = item;
                    }
                }
                else
                {
                    minElement = item;
                    hasValue = true;
                }
            }
            return minElement;
        }
        public static TSource MaxElement<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            if (source.Any())
                return MaxOrDefaultElement(source, comparer);
            throw new InvalidOperationException("序列 source 中不包含任何元素");
        }
        public static TSource MaxOrDefaultElement<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var maxElement = default(TSource);
            comparer = comparer ?? Comparer<TSource>.Default;
            var hasValue = false;
            foreach (var item in source)
            {
                if (hasValue)
                {
                    if (comparer.Compare(item, maxElement) > 0)
                    {
                        maxElement = item;
                    }
                }
                else
                {
                    maxElement = item;
                    hasValue = true;
                }
            }
            return maxElement;
        }

        public static IEnumerable<TSource> MaxElements<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector, IComparer<TValue> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (selector == null)
                throw new ArgumentNullException("selector");

            if (source.Any())
            {
                comparer = comparer ?? Comparer<TValue>.Default;
                var maxElement = source.First();
                var maxValue = selector(maxElement);
                var result = new List<TSource> { maxElement };
                var skip = true;
                foreach (var item in source)
                {
                    if (skip)
                    {
                        skip = false;
                        continue;
                    }
                    var curValue = selector(item);
                    var compareResult = comparer.Compare(curValue, maxValue);
                    if (compareResult > 0)
                    {
                        result.Clear();
                        result.Add(item);
                        maxValue = curValue;
                    }
                    else if (compareResult == 0)
                    {
                        result.Add(item);
                    }
                }
                return result;
            }
            return Enumerable.Empty<TSource>();
        }
        public static IEnumerable<TSource> MinElements<TSource, TValue>(this IEnumerable<TSource> source, Func<TSource, TValue> selector, IComparer<TValue> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (selector == null)
                throw new ArgumentNullException("selector");

            if (source.Any())
            {
                comparer = comparer ?? Comparer<TValue>.Default;
                var minElement = source.First();
                var minValue = selector(minElement);
                var result = new List<TSource> { minElement };
                var skip = true;
                foreach (var item in source)
                {
                    if (skip)
                    {
                        skip = false;
                        continue;
                    }
                    var curValue = selector(item);
                    var compareResult = comparer.Compare(curValue, minValue);
                    if (compareResult < 0)
                    {
                        result.Clear();
                        result.Add(item);
                        minValue = curValue;
                    }
                    else if (compareResult == 0)
                    {
                        result.Add(item);
                    }
                }
                return result;
            }
            return Enumerable.Empty<TSource>();
        }

        public static IEnumerable<TSource> SelectElements<TSource>(this IEnumerable<IEnumerable<TSource>> source, Func<TSource, bool> predicate = null)
        {
            return (from sitem in source
                    from item in sitem
                    where predicate == null || predicate(item)
                    select item).ToList();
        }

        public static IEnumerable<TSource> RemoveNullElements<TSource>(this IEnumerable<TSource> source) where TSource : class
        {
            return source.Where(item => item != null).ToList();
        }
        public static IEnumerable<string> RemoveNullOrEmptyElements(this IEnumerable<string> source)
        {
            return source.Where(item => !string.IsNullOrEmpty(item)).ToList();
        }
        public static IEnumerable<string> RemoveNullOrWhiteSpaceElements(this IEnumerable<string> source)
        {
            return source.Where(item => !string.IsNullOrWhiteSpace(item)).ToList();
        }
        /// <summary>
        /// 分页数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="items">数据集合</param>
        /// <param name="pageNo">从1开始</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public static IEnumerable<T> ToPaged<T>(this IEnumerable<T> items, int pageIndex, int pageSize)
        {
            return items.Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }
        /// <summary>
        /// 为当前序列中的每一个元素执行指定的操作。
        /// </summary>
        /// <typeparam name="TSource">序列中元素的类型。</typeparam>
        /// <param name="sequence">要为元素执行操作的序列。</param>
        /// <param name="action">要为元素执行的操作。</param>
        public static void For<TSource>(this IEnumerable<TSource> sequence, Action<TSource, int> action)
        {
            if (sequence == null)
                throw new ArgumentNullException("sequence");
            if (action != null)
            {
                var array = sequence.ToArray();
                for (var i = 0; i < array.Length; i++)
                {
                    action(array[i], i);
                }
            }
        }

        private class EmptyReadOnlyCollection<TSource>
        {
            internal static readonly ReadOnlyCollection<TSource> Empty = new List<TSource>().AsReadOnly();
        }

        public static DataTable ToTable<TSource, TMember>(this IEnumerable<TSource> src, Func<TSource, IEnumerable<Tuple<string, TMember>>> fn)
        {
            var table = new DataTable();
            var data = src.Select(fn).ToArray();
            if (!data.Any()) { return null; }

            var fields = data.First().Select(t => t.Item1);
            foreach (var field in fields)
            {
                table.Columns.Add(field);
            }
            foreach (var item in data)
            {
                var row = table.NewRow();
                foreach (var col in item)
                {
                    row[col.Item1] = col.Item2;
                }
                table.Rows.Add(row);
            }
            return table;
        }
        public static DataTable ToTable(this IEnumerable<Guid> src, string colName)
        {
            return src.ToTable(item => new[] { new Tuple<string, Guid>(colName, item) });
        }
        public static DataTable ToTable(this IEnumerable<string> src, string colName)
        {
            return src.ToTable(item => new[] { new Tuple<string, string>(colName, item) });
        }



        #region  ext
        /// <summary>
        /// 为所有元素调用指定的方法
        /// </summary>
        /// <typeparam name="T"> 元素类型 </typeparam>
        /// <param name="set"> 被扩展的 IEnumerable&lt;T&gt; </param>
        /// <param name="action"> 要为元素调用的方法 </param>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (action == null)
                throw new ArgumentNullException("action");
            foreach (var item in source) action(item);
        }

        /// <summary>
        /// 为所有元素调用指定的方法，除非它满足排除条件
        /// </summary>
        /// <typeparam name="T"> 元素类型 </typeparam>
        /// <param name="set"> 被扩展的 IEnumerable&lt;T&gt; </param>
        /// <param name="action"> 要为元素调用的方法 </param>
        /// <param name="except"> 排除条件谓词 </param>
        public static void ForEachExcept<T>(this IEnumerable<T> set, Action<T> action, Func<T, bool> except)
        {
            if (action == null) return;
            foreach (T item in set.Where(item => except == null || !except(item)))
            {
                action(item);
            }
        }

        /// <summary>
        /// 为所有元素调用指定的方法，在满足指定条件时终止循环
        /// </summary>
        /// <typeparam name="T"> 元素类型 </typeparam>
        /// <param name="set"> 被扩展的 IEnumerable&lt;T&gt; </param>
        /// <param name="action"> 要为元素调用的方法 </param>
        /// <param name="predicate"> 终止循环的条件 </param>
        public static void ForEachUntil<T>(this IEnumerable<T> set, Action<T> action, Func<T, bool> predicate)
        {
            if (action == null) return;
            if (predicate == null)
            {
                ForEach(set, action);
            }
            else
            {
                foreach (T item in set)
                {
                    if (predicate(item))
                    {
                        return;
                    }
                    action(item);
                }
            }
        }

        /// <summary>
        /// 向集合中添加元素
        /// </summary>
        /// <typeparam name="T"> 集合中元素的类型 </typeparam>
        /// <param name="set"> 被扩展的属性 </param>
        /// <param name="item"> 要添加的元素 </param>
        /// <returns> 成功添加返回 true,失败返回 false </returns>
        public static bool Add<T>(this ICollection<T> set, T item)
        {
            try
            {
                set.Add(item);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 向集合中添加多个元素
        /// </summary>
        /// <typeparam name="T"> 集合中元素的类型 </typeparam>
        /// <param name="set"> 被扩展的属性 </param>
        /// <param name="items"> 要添加的元素列表 </param>
        /// <returns> 返回成功添加的元素数量 </returns>
        public static int AddRange<T>(this ICollection<T> set, IEnumerable<T> items)
        {
            if (set.IsReadOnly)
            {
                throw new InvalidOperationException("只读集合（属性 IsReadOnly == true），不允许添加元素。");
            }
            return items == null ? 0 : items.Count(item => Add(set, item));
        }

        /// <summary>
        /// 更新所有元素
        /// </summary>
        /// <typeparam name="T"> 元素类型 </typeparam>
        /// <param name="set"> 被扩展的 IEnumerable&lt;T&gt; </param>
        /// <param name="action"> 更新方法 </param>
        /// <returns> 返回更新的元素数量 </returns>
        public static int Update<T>(this IEnumerable<T> set, Action<T> action)
        {
            return Update(set, action, null);
        }

        /// <summary>
        /// 更新满足指定的条件元素
        /// </summary>
        /// <typeparam name="T"> 元素类型 </typeparam>
        /// <param name="set"> 被扩展的 IEnumerable&lt;T&gt; </param>
        /// <param name="action"> 更新方法 </param>
        /// <param name="predicate"> 条件谓词 </param>
        /// <returns> 返回更新的元素数量 </returns>
        public static int Update<T>(this IEnumerable<T> set, Action<T> action, Func<T, bool> predicate)
        {
            if (action == null)
            {
                return 0;
            }
            IEnumerable<T> items = set.ToList().Where(item => predicate != null && predicate(item));
            int count = 0;
            foreach (T item in items)
            {
                action(item);
                count++;
            }
            return count;
        }

        /// <summary>
        /// 删除满足条件的元素
        /// </summary>
        /// <typeparam name="T"> 元素类型 </typeparam>
        /// <param name="set"> 被扩展的 IList&lt;T&gt; </param>
        /// <param name="predicate"> 条件谓词 </param>
        /// <returns> 返回删除的元素数量 </returns>
        /// <remarks>
        /// 如果条件谓词 predicate 为 null，将删除所有元素
        /// </remarks>
        public static int Delete<T>(this ICollection<T> set, Func<T, bool> predicate)
        {
            // ICollection 里面的元素没有顺序，不能通过下标访问，所以只能通过 foreach 遍历
            // foreach 在进行迭代的时候不能删除元素（并且 ICollection 接口也不提供删除元素的方法）
            // 所以以下是比较恶心的实现：
            // 1、遍历所有元素，将不满足条件的元素添加到新的列表
            // 2、清空原 set 中的元素
            // 3、将保留下来的元素重新添加回原 set
            if (set.IsReadOnly)
            {
                throw new InvalidOperationException("只读集合（属性 IsReadOnly == true），不允许删除元素。");
            }
            int count = set.Count;
            ICollection<T> retains = predicate == null ? set : set.Where(item => !predicate(item)).ToList();
            set.Clear();
            return predicate == null ? count : count - set.AddRange(retains);
        }

        /// <summary>
        /// 查找元素在序列中首次出现的位置
        /// </summary>
        /// <typeparam name="T"> 序列中元素的类型 </typeparam>
        /// <param name="set"> 被扩展的序列 </param>
        /// <param name="item"> 要查找的元素 </param>
        /// <returns> 元素在序列中首次出现的位置。如果元素不在序列中，返回 -1. </returns>
        public static int IndexOf<T>(this IEnumerable<T> set, T item)
        {
            for (int pos = 0; pos < set.Count(); pos++)
            {
                if (set.ElementAt(pos).Equals(item))
                {
                    return pos;
                }
            }
            return -1;
        }

        /// <summary>
        /// 查找元素的匹配项在序列中首次出现的位置
        /// </summary>
        /// <typeparam name="T"> 序列中元素的类型 </typeparam>
        /// <param name="set"> 被扩展的序列 </param>
        /// <param name="item"> 要查找的元素 </param>
        /// <param name="comparer"> 用于比较元素相等的比较器 </param>
        /// <returns> 元素在序列中首次出现的位置。如果元素不在序列中，返回 -1. </returns>
        public static int IndexOf<T>(this IEnumerable<T> set, T item, IEqualityComparer<T> comparer)
        {
            if (set == null)
                return 0;

            if (comparer == null)
            {
                return IndexOf(set, item);
            }
            for (int pos = 0; pos < set.Count(); pos++)
            {
                if (comparer.Equals(set.ElementAt(pos), item))
                {
                    return pos;
                }
            }
            return -1;
        }
        /// <summary>
        /// </summary>
        /// <param name="set"> </param>
        /// <param name="op"> </param>
        /// <returns> </returns>

        #endregion

        public static IEnumerable<T> Sort<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> fnItemsBeforeMe)
        {
            return Sort(items, fnItemsBeforeMe, null);
        }

        public static IEnumerable<T> Sort<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> fnItemsBeforeMe, IEqualityComparer<T> comparer)
        {
            HashSet<T> seen = comparer != null ? new HashSet<T>(comparer) : new HashSet<T>();
            HashSet<T> done = comparer != null ? new HashSet<T>(comparer) : new HashSet<T>();
            var result = new List<T>();
            foreach (T item in items)
            {
                SortItem(item, fnItemsBeforeMe, seen, done, result);
            }
            return result;
        }

        private static void SortItem<T>(T item, Func<T, IEnumerable<T>> fnItemsBeforeMe, HashSet<T> seen, HashSet<T> done, List<T> result)
        {
            if (!done.Contains(item))
            {
                if (seen.Contains(item))
                {
                    throw new InvalidOperationException("Cycle in topological sort");
                }
                seen.Add(item);
                IEnumerable<T> itemsBefore = fnItemsBeforeMe(item);
                if (itemsBefore != null)
                {
                    foreach (T itemBefore in itemsBefore)
                    {
                        SortItem(itemBefore, fnItemsBeforeMe, seen, done, result);
                    }
                }
                result.Add(item);
                done.Add(item);
            }
        }
    }
}