#region File Comments

// ////////////////////////////////////////////////////////////////////////////////////////////////
// file：Izual.Linq.ScopedDictionary.cs
// description：
// 
// create by：Izual ,2012/07/03
// last modify：Izual ,2012/07/05
// ////////////////////////////////////////////////////////////////////////////////////////////////

#endregion

using System.Collections.Generic;

namespace YSL.Common.Utility
{
    /// <summary>
    /// 上下文字典 链表
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class ScopedDictionary<TKey, TValue> {
        private readonly Dictionary<TKey, TValue> map;
        private readonly ScopedDictionary<TKey, TValue> previous;

        public ScopedDictionary(ScopedDictionary<TKey, TValue> previous) {
            this.previous = previous;
            map = new Dictionary<TKey, TValue>();
        }

        public ScopedDictionary(ScopedDictionary<TKey, TValue> previous, IEnumerable<KeyValuePair<TKey, TValue>> pairs) : this(previous) {
            foreach(KeyValuePair<TKey, TValue> p in pairs) {
                map.Add(p.Key, p.Value);
            }
        }

        public void Add(TKey key, TValue value) {
            map.Add(key, value);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            for(ScopedDictionary<TKey, TValue> scope = this; scope != null; scope = scope.previous) {
                if(scope.map.TryGetValue(key, out value))
                    return true;
            }
            value = default(TValue);
            return false;
        }

        public bool ContainsKey(TKey key) {
            for(ScopedDictionary<TKey, TValue> scope = this; scope != null; scope = scope.previous) {
                if(scope.map.ContainsKey(key))
                    return true;
            }
            return false;
        }
    }
}