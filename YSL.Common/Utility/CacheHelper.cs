using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
namespace YSL.Common.Utility
{
    /// <summary>
    /// HttpContext.Cache缓存 帮助类
    /// </summary>
    public class CacheHelper
    {
        /// <summary>
        /// 创建缓存项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Insert(string key, object value)
        {
            HttpContext.Current.Cache.Insert(key, value);
        }
        /// <summary>
        /// 创建缓存项的文件依赖
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="fileName">文件绝对路径</param>
        public static void Insert(string key, object value, string fileName)
        {
            CacheDependency dep = new CacheDependency(fileName);
            HttpContext.Current.Cache.Insert(key, value, dep);
        }
        /// <summary>
        /// 创建缓存项过期时间（分钟）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vslue"></param>
        /// <param name="expires"></param>
        public static void Insert(string key, object vslue, int expires)
        {
            HttpContext.Current.Cache.Insert(key, vslue, null, Cache.NoAbsoluteExpiration, new TimeSpan(0, expires, 0));
        }

        public static object Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }
            return HttpContext.Current.Cache.Get(key);
        }

        public static T Get<T>(string key)
        {
            object obj = Get(key);
            return obj == null ? default(T) : (T)obj;
        }
        public static object Remove(string key)
        {
            return HttpContext.Current.Cache.Remove(key);
        }
    }
}
