using System;
using System.Collections.Generic;
using System.Collections;
using System.Web.Caching;
using System.Web;

namespace YSL.Framework.Cache
{
    public class Caches
    {
        /// <summary>
        /// 建立缓存
        /// </summary>
        /// <param name="key">用于引用该项的缓存键</param>
        /// <param name="value">要添加到缓存的项</param>
        /// <param name="dependency">该项的文件依赖项或缓存键依赖项</param>
        /// <param name="absoluteExpiration">所添加对象将过期并被从缓存中移除的时间</param>
        /// <param name="slidingExpiration">缓存滑动时间</param>
        /// <param name="priority">对象的相对成本，由 CacheItemPriority 枚举表示。缓存在退出对象时使用该值；具有较低成本的对象在具有较高成本的对象之前被从缓存移除。 </param>
        /// <param name="onRemoveCallback">在从缓存中移除对象时所调用的委托</param>
        /// <returns></returns>
        public static object TryAddCaChe(string key, object value, ICacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback onRemoveCallback)
        {
            if (HttpRuntime.Cache[key] == null && value != null)
            {
                if (dependency != null)
                {
                    return HttpRuntime.Cache.Add(key, value, dependency.GetDependency(), absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
                }
                else
                {
                    return HttpRuntime.Cache.Add(key, value, null, absoluteExpiration, slidingExpiration, priority, onRemoveCallback);
                }
            }
            return null;
        }

        /// <summary>
        /// 建立缓存
        /// </summary>
        /// <param name="key">用于引用该项的缓存键</param>
        /// <param name="value">要添加到缓存的项</param>
        /// <param name="dependency">该项的文件依赖项或缓存键依赖项</param>
        /// <param name="absoluteExpiration">所添加对象将过期并被从缓存中移除的时间</param>
        /// <returns></returns>
        public static object TryAddCaChe(string key, object value, ICacheDependency dependency, DateTime absoluteExpiration)
        {
            if (HttpRuntime.Cache[key] == null && value != null)
            {
                if (dependency != null)
                {
                    return HttpRuntime.Cache.Add(key, value, dependency.GetDependency(), absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                }
                else
                {
                    return HttpRuntime.Cache.Add(key, value, null, absoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.High, null);
                }
            }
            return null;
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object TryRemoveCache(string key)
        {
            if (HttpRuntime.Cache[key] != null)
            {
                return HttpRuntime.Cache.Remove(key);
            }
            return null;
        }

        /// <summary>
        /// 移除键中带某关键字的缓存
        /// </summary>
        /// <param name="keyInclude"></param>
        public static void RemoveMultiCache(string keyInclude)
        {
            IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                if (cacheEnum.Key.ToString().IndexOf(keyInclude) >= 0)
                {
                    HttpRuntime.Cache.Remove(cacheEnum.Key.ToString());
                }
            }
        }

        /// <summary>
        /// 移除键中带前缀某关键字的缓存
        /// </summary>
        /// <param name="keyInclude"></param>
        public static void RemovePrefixCache(string keyInclude)
        {
            IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                if (cacheEnum.Key.ToString().StartsWith(keyInclude))
                {
                    HttpRuntime.Cache.Remove(cacheEnum.Key.ToString());
                }
            }
        }

        /// <summary>
        /// 移除所有缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();
            while (cacheEnum.MoveNext())
            {
                HttpRuntime.Cache.Remove(cacheEnum.Key.ToString());
            }
        }
    }
}
