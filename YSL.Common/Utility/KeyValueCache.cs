using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace YSL.Common.Utility
{
    /// <summary>
    /// 用于存储键值对的缓存，提供共享读和独占写的锁定。
    /// </summary>
    /// <typeparam name="TKey">键类型。</typeparam>
    /// <typeparam name="TValue">值类型。</typeparam>
    public class KeyValueCache<TKey, TValue> {
        private readonly Dictionary<TKey, TValue> cache;
        private readonly ReaderWriterLockSlim locker = new ReaderWriterLockSlim();


        /// <summary>
        /// 初始化 KeyValueCache 类型的新实例。可选择设置指定的键相等比较器，和读写操作的等待超时时间。
        /// </summary>
        /// <param name="comparer">用于判断键相等的比较器。</param>
        /// <param name="timeout">读写等待的超时时间。(默认为 -1，表示永久等待无超时；设置为 0 表示不等待。)</param>
        public KeyValueCache(IEqualityComparer<TKey> comparer, int timeout = -1) {
            cache = comparer == null ? new Dictionary<TKey, TValue>() : new Dictionary<TKey, TValue>(comparer);
            Timeout = timeout;
        }

        /// <summary>
        /// 初始化 KeyValueCache 类型的新实例。可选择指定读写操作的等待超时时间。
        /// </summary>
        /// <param name="timeout">读写等待的超时时间。(默认为 -1，表示永久等待无超时；设置为 0 表示不等待。)</param>
        public KeyValueCache(int timeout = -1) : this(null, timeout) { }

        /// <summary>
        /// 获取/设置读写操作的等待超时时间。
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 获取指定键对应的值。
        /// </summary>
        /// <param name="key">要获取对应值的键。</param>
        /// <returns>返回获取到的值。</returns>
        public TValue this[TKey key] {
            get {
                if (locker.TryEnterReadLock(Timeout)) {
                    try {
                        return cache[key];
                    }
                    finally {
                        locker.ExitReadLock();
                    }
                }
                throw new TimeoutException();
            }
            set { SetValue(key, value); }
        }

        /// <summary>
        /// 获取指定键对应的值，如果指定键不存在则新建，并设值为 defaultValue 指定的值。
        /// </summary>
        /// <param name="key">要获取对应值的键。</param>
        /// <param name="defaultValue">为新建的键设置的值。</param>
        /// <returns>返回指定键对应的值。</returns>
        public TValue this[TKey key, TValue defaultValue] {
            get {
                if (!cache.ContainsKey(key)) {
                    SetValue(key, defaultValue);
                }
                return this[key];
            }
        }

        /// <summary>
        /// 获取指定键对应的值，如果指定键不存在则新建，并设值为 fnNewValue 计算的结果。
        /// </summary>
        /// <param name="key">要获取对应值的键。</param>
        /// <param name="fnNewValue">用于计算为新建的键设置的值的委托。</param>
        /// <returns>返回指定键对应的值。</returns>
        public TValue this[TKey key, Func<TKey, TValue> fnNewValue] {
            get {
                if (!cache.ContainsKey(key)) {
                    SetValue(key, fnNewValue);
                }
                return this[key];
            }
        }

        /// <summary>
        /// 获取满足指定条件的第一个键对应的值。
        /// </summary>
        /// <param name="predicate">要获取对应值的键。</param>
        /// <returns>返回指定键对应的值。</returns>
        public TValue this[Func<TKey, bool> predicate] {
            get {
                if (locker.TryEnterReadLock(Timeout)) {
                    try {
                        if (cache.Keys.Any(predicate)) {
                            return cache[cache.Keys.First(predicate)];
                        }
                        throw new ArgumentException("不存在满足指定条件的键。");
                    }
                    finally {
                        locker.ExitReadLock();
                    }
                }
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// 确定缓存中是否包含指定的键。
        /// </summary>
        /// <param name="key">要判断的键。</param>
        /// <returns>如果缓存中包含指定键，返回 true；否则返回 false。</returns>
        public bool ContainsKey(TKey key) {
            if (locker.TryEnterReadLock(Timeout)) {
                try {
                    return cache.ContainsKey(key);
                }
                finally {
                    locker.ExitReadLock();
                }
            }
            throw new TimeoutException();
        }

        /// <summary>
        /// 确定缓存中是否包含满足指定条件的键。
        /// </summary>
        /// <param name="predicate">判断条件谓词。</param>
        /// <returns>如果缓存中包含指定键，返回 true；否则返回 false。</returns>
        public bool ContainsKey(Func<TKey,bool> predicate) {
            if (locker.TryEnterReadLock(Timeout)) {
                try {
                    return cache.Keys.Any(predicate);
                }
                finally {
                    locker.ExitReadLock();
                }
            }
            throw new TimeoutException();
        }

        /// <summary>
        /// 确定缓存中是否包含指定的值。
        /// </summary>
        /// <param name="value">要判断的值。</param>
        /// <returns>如果缓存中包含指定值，返回 true；否则返回 false。</returns>
        public bool ContainsValue(TValue value) {
            if (locker.TryEnterReadLock(Timeout)) {
                try {
                    return cache.ContainsValue(value);
                }
                finally {
                    locker.ExitReadLock();
                }
            }
            throw new TimeoutException();
        }

        /// <summary>
        /// 确定缓存中是否包含满足指定条件的值。
        /// </summary>
        /// <param name="predicate">判断条件谓词。</param>
        /// <returns>如果缓存中包含指定值，返回 true；否则返回 false。</returns>
        public bool ContainsValue(Func<TValue,bool> predicate) {
            if (locker.TryEnterReadLock(Timeout)) {
                try {
                    return cache.Values.Any(predicate);
                }
                finally {
                    locker.ExitReadLock();
                }
            }
            throw new TimeoutException();
        }


        /// <summary>
        /// 获取缓存中项目的数量。
        /// </summary>
        public int Count {
            get {
                if (locker.TryEnterReadLock(Timeout)) {
                    try {
                        return cache.Count;
                    }
                    finally {
                        locker.ExitReadLock();
                    }
                }
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// 清除缓存中所有项目。
        /// </summary>
        public void Clear() {
            if (locker.TryEnterWriteLock(Timeout)) {
                try {
                    cache.Clear();
                }
                finally {
                    locker.ExitWriteLock();
                }
            }
            else {
                throw new TimeoutException();
            }
        }

      
        /// <summary>
        /// 为指定的键设置对应的值，如果不存在指定的键则新建。
        /// </summary>
        /// <param name="key">要设值的键。</param>
        /// <param name="value">要设置的值。</param>
        private void SetValue(TKey key, TValue value) {
            if (locker.TryEnterWriteLock(Timeout)) {
                try {
                    if (!cache.ContainsKey(key)) {
                        cache.Add(key, value);
                    }
                    else {
                        cache[key] = value;
                    }
                }
                finally {
                    locker.ExitWriteLock();
                }
            }
            else {
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// 为指定的键设置对应的值，如果不存在指定的键则新建。
        /// </summary>
        /// <param name="key">要设值的键。</param>
        /// <param name="fnValue">用于计算要设置的值的委托。</param>
        private void SetValue(TKey key, Func<TKey, TValue> fnValue) {
            if (locker.TryEnterWriteLock(Timeout)) {
                try {
                    if (!cache.ContainsKey(key)) {
                        cache.Add(key, fnValue(key));
                    }
                    else {
                        cache[key] = fnValue(key);
                    }
                }
                finally {
                    locker.ExitWriteLock();
                }
            }
            else {
                throw new TimeoutException();
            }
        }
    }
}
