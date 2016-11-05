using NLog;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Common.Log;

//使用和配置
//<section name="RedisConfig" type="PinMall.Core.Config.RedisConfigInfo, PinMall.Core" />
//<!--redis配置节点 WriteServerList、ReadServerList多个服务器用逗号分隔-->
// <RedisConfig WriteServerList="" ReadServerList="" MaxWritePoolSize="10" MaxReadPoolSize="10" AutoStart="true" LocalCacheTime="180" Password="">
// </RedisConfig>

namespace YSL.Framework.Cache.Redis
{
    /// <summary>
    /// Redis客户端帮助类
    /// </summary>
    public class RedisHelper
    {
        private static string RedisReadWritePath = "";
        private static string RedisReadPath = "";
        public static PooledRedisClientManager prcm = null;
        private static NLog.Logger logger = LogBuilder.NLogger;
        static RedisHelper()
        {
            RedisConfigInfoSection redis = RedisConfigInfoSection.GetConfig();
            RedisReadWritePath = redis.WriteServerList;
            RedisReadPath = redis.ReadServerList;
            prcm = CreateManager(new string[] { RedisReadWritePath }, new string[] { RedisReadPath }, redis.MaxWritePoolSize, redis.MaxReadPoolSize);
        }

        #region -- 连接信息 --
        private static PooledRedisClientManager CreateManager(string[] readWriteHosts, string[] readOnlyHosts, int maxWritePoolSize, int maxReadPoolSize)
        {
            var RedisClientManagerConfig = new RedisClientManagerConfig
            {
                MaxWritePoolSize = maxWritePoolSize, // “写”链接池链接数 
                MaxReadPoolSize = maxReadPoolSize, // “读”链接池链接数 
                AutoStart = true,
            };
            // 支持读写分离，均衡负载 
            return new PooledRedisClientManager(readWriteHosts, readOnlyHosts, RedisClientManagerConfig);
        }
        #endregion

        #region -- Item --
        /// <summary>
        /// 设置单体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public static bool Item_Set<T>(string key, T t)
        {
            try
            {
                using (IRedisClient redis = prcm.GetClient())
                {
                    return redis.Set<T>(key, t, new TimeSpan(1, 0, 0));
                }
            }
            catch (Exception ex)
            {
                logger.Error("Item_Set:", ex);
            }
            return false;
        }

        /// <summary>
        /// 获取单体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Item_Get<T>(string key) // where T : class
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                return redis.Get<T>(key);
            }
        }

        /// <summary>
        /// 获取多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static IDictionary<string, T> MGet<T>(params string[] keys) where T : class
        {
            if (keys == null || keys.Length == 0)
            {
                return new Dictionary<string, T>();
            }
            using (IRedisClient redis = prcm.GetClient())
            {
                return redis.GetAll<T>(keys);
            }
        }

        /// <summary>
        /// 设置多个元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void MSet<T>(IDictionary<string, T> di) where T : class
        {
            if (di == null || di.Count == 0)
            {
                return;
            }
            using (IRedisClient redis = prcm.GetClient())
            {
                redis.SetAll(di);
            }
        }

        /// <summary>
        /// 移除单体
        /// </summary>
        /// <param name="key"></param>
        public static bool Item_Remove(string key)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                return redis.Remove(key);
            }
        }

        #endregion

        /// <summary>
        /// 得到所有缓存
        /// 张森
        /// 2015/8/4
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllKeys<T>()
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                return redisTypedClient.GetAllKeys();
            }
        }
        #region -- List --

        public static void List_Add<T>(string key, T t)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                redisTypedClient.AddItemToList(redisTypedClient.Lists[key], t);
            }
        }

        public static bool List_Remove<T>(string key, T t)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                return redisTypedClient.RemoveItemFromList(redisTypedClient.Lists[key], t) > 0;
            }
        }
        public static void List_RemoveAll<T>(string key)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                redisTypedClient.Lists[key].RemoveAll();
            }
        }

        public static long List_Count(string key)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                return redis.GetListCount(key);
            }
        }

        public static List<T> List_GetRange<T>(string key, int start, int count)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var c = redis.GetTypedClient<T>();
                return c.Lists[key].GetRange(start, start + count - 1);
            }
        }

        public static bool List_Exist<T>(string key)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var c = redis.GetTypedClient<T>();
                return c.ContainsKey(key);
            }
        }

        public static List<T> List_GetList<T>(string key)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var c = redis.GetTypedClient<T>();
                return c.Lists[key].GetRange(0, c.Lists[key].Count);
            }
        }

        public static List<T> List_GetList<T>(string key, int pageIndex, int pageSize)
        {
            int start = pageSize * (pageIndex - 1);
            return List_GetRange<T>(key, start, pageSize);
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void List_SetExpire(string key, DateTime datetime)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }


        #endregion

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiredTime"></param>
        public static void Expire(string key, TimeSpan expiredTime)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                redis.ExpireEntryIn(key, expiredTime);
            }
        }
        /// <summary>
        /// 判断元素是否存在
        /// </summary>
        /// <param name="key"></param>
        public static bool Exists(string key)
        {
            using (RedisClient redis = (RedisClient)prcm.GetClient())
            {
                return redis.Exists(key) > 0;
            }
        }

        #region -- Set --
        public static void Set_AddRange<T>(string key, params T[] t)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                foreach (var item in t)
                {
                    redisTypedClient.AddItemToSet(redisTypedClient.Sets[key], item);
                }
            }
        }
        public static void Set_Add<T>(string key, T t)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                redisTypedClient.Sets[key].Add(t);
            }
        }
        public static HashSet<T> Set_GetAll<T>(string key)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                return redisTypedClient.Sets[key].GetAll();
            }
        }
        public static bool Set_Contains<T>(string key, T t)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                return redisTypedClient.Sets[key].Contains(t);
            }
        }
        public static bool Set_Remove<T>(string key, T t)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var redisTypedClient = redis.GetTypedClient<T>();
                return redisTypedClient.Sets[key].Remove(t);
            }
        }
        #endregion


        #region -- Hash --
        /// <summary>
        /// 判断某个数据是否已经被缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Exist<T>(string key, string dataKey)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                return redis.HashContainsEntry(key, dataKey);
            }
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Set<T>(string key, string dataKey, T t)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.SetEntryInHash(key, dataKey, value);
            }
        }

        /// <summary>
        /// 存储数据到hash表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static T JsonTest<T>(string key, string dataKey)
        {
            string value = RedisHelper.Hash_Get<string>(key, dataKey);
            T t = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);
            //using (IRedisClient redis = prcm.GetClient())
            //{

            //}
            return t;
        }

        /// <summary>
        /// 移除hash中的某值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Remove(string key, string dataKey)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                return redis.RemoveEntryFromHash(key, dataKey);
            }
        }
        /// <summary>
        /// 移除整个hash
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static bool Hash_Remove(string key)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                return redis.Remove(key);
            }
        }
        /// <summary>
        /// 从hash表获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataKey"></param>
        /// <returns></returns>
        public static T Hash_Get<T>(string key, string dataKey)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                string value = redis.GetValueFromHash(key, dataKey);
                return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(value);
            }
        }
        /// <summary>
        /// 获取整个hash的数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> Hash_GetAll<T>(string key)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var list = redis.GetHashValues(key);
                if (list != null && list.Count > 0)
                {
                    List<T> result = new List<T>();
                    foreach (var item in list)
                    {
                        var value = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                        result.Add(value);
                    }
                    return result;
                }
                return null;
            }
        }
        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void Hash_SetExpire(string key, DateTime datetime)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }
        #endregion



        #region -- SortedSet --
        /// <summary>
        ///  添加数据到 SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <param name="score"></param>
        public static bool SortedSet_Add<T>(string key, T t, double score)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.AddItemToSortedSet(key, value, score);
            }
        }
        /// <summary>
        /// 移除数据从SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool SortedSet_Remove<T>(string key, T t)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                string value = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
                return redis.RemoveItemFromSortedSet(key, value);
            }
        }

        /// <summary>
        /// 移除数据从SortedSet
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public static void SortedSet_RemoveWithScore(string key, long score)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                redis.RemoveRangeFromSortedSetByScore(key, score, score);
            }
        }
        /// <summary>
        /// 修剪SortedSet
        /// </summary>
        /// <param name="key"></param>
        /// <param name="size">保留的条数</param>
        /// <returns></returns>
        public static long SortedSet_Trim(string key, int size)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                return redis.RemoveRangeFromSortedSet(key, size, 9999999);
            }
        }
        /// <summary>
        /// 获取SortedSet的长度
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static long SortedSet_Count(string key)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                return redis.GetSortedSetCount(key);
            }
        }

        /// <summary>
        /// 获取SortedSet的分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<T> SortedSet_GetList<T>(string key, int pageIndex, int pageSize)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var list = redis.GetRangeFromSortedSet(key, (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                if (list != null && list.Count > 0)
                {
                    List<T> result = new List<T>();
                    foreach (var item in list)
                    {
                        var data = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                        result.Add(data);
                    }
                    return result;
                }
            }
            return null;
        }


        /// <summary>
        /// 获取SortedSet的分页数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<T> SortedSet_GetListDesc<T>(string key, int pageIndex, int pageSize)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var list = redis.GetRangeFromSortedSetDesc(key, (pageIndex - 1) * pageSize, pageIndex * pageSize - 1);
                if (list == null)
                {
                    return null;
                }
                List<T> result = new List<T>();
                foreach (var item in list)
                {
                    var data = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                    result.Add(data);
                }
                return result;
            }
        }



        /// <summary>
        /// 获取SortedSet的全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<T> SortedSet_GetListALL<T>(string key)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                var list = redis.GetRangeFromSortedSet(key, 0, 9999999);
                if (list != null && list.Count > 0)
                {
                    List<T> result = new List<T>();
                    foreach (var item in list)
                    {
                        var data = ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(item);
                        result.Add(data);
                    }
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// 设置缓存过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="datetime"></param>
        public static void SortedSet_SetExpire(string key, DateTime datetime)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                redis.ExpireEntryAt(key, datetime);
            }
        }

        //public static double SortedSet_GetItemScore<T>(string key,T t)
        //{
        //    using (IRedisClient redis = prcm.GetClient())
        //    {
        //        var data = ServiceStack.Text.JsonSerializer.SerializeToString<T>(t);
        //        return redis.GetItemScoreInSortedSet(key, data);
        //    }
        //    return 0;
        //}

        #endregion

        #region -- Redis缓存处理 --


        /// <summary>
        /// 加入当前对象到缓存中
        /// </summary>
        /// <param name="key">对象的键值</param>
        /// <param name="value">缓存的对象</param>
        /// <param name="expire">到期时间,单位:秒</param>
        public static bool SetObject<T>(string key, T value, int expire = 0, IRedisClient redis = null, bool isClose = true)
        {
            try
            {
                if (redis == null)
                {
                    redis = GetClient();
                }
                bool setResult = false;
                if (expire > 0)
                {
                    setResult = redis.Set<T>(key, value, DateTime.Now.AddSeconds(expire));
                }
                else
                {
                    setResult = redis.Set<T>(key, value);
                }
                if (isClose)
                {
                    Close(redis);
                }
                return setResult;
            }
            catch (Exception ex)
            {
                logger.Error("SetObject:" + key + value, ex);
                return false;
            }
        }

        /// <summary>
        /// 加入当前对象到缓存中
        /// </summary>
        /// <param name="key">对象的键值</param>
        /// <param name="value">缓存的对象</param>
        /// <param name="expire">到期时间,单位:秒</param>
        public static bool AddObject<T>(string key, T value, int expire = 0, IRedisClient redis = null, bool isClose = true)
        {
            try
            {
                if (redis == null)
                {
                    redis = GetClient();
                }
                bool addResult = false;
                if (expire > 0)
                {
                    addResult = redis.Add<T>(key, value, DateTime.Now.AddSeconds(expire));
                }
                else
                {
                    addResult = redis.Add<T>(key, value);
                }
                if (isClose)
                {
                    Close(redis);
                }
                return addResult;
            }
            catch (Exception ex)
            {
                logger.Error("AddObject:" + key + value, ex);
                return false;
            }
        }



        /// <summary>
        /// 返回一个指定的对象
        /// </summary>
        /// <param name="key">对象的关键字</param>
        /// <returns>对象</returns>
        public static T RetrieveObject<T>(string key, IRedisClient redis = null, bool isClose = true)
        {

            try
            {
                if (redis == null)
                {
                    redis = GetClient();
                }
                T obj = redis.Get<T>(key);
                if (isClose)
                {
                    Close(redis);
                }
                return obj;
            }
            catch (Exception ex)
            {
                logger.Error("RetrieveObject:" + key, ex);
                return default(T);
            }
        }

        /// <summary>
        /// 移除指定ID的对象
        /// </summary>
        /// <param name="objId"></param>
        public static bool RemoveObject(string key, IRedisClient redis = null, bool isClose = true)
        {
            try
            {
                if (redis == null)
                {
                    redis = GetClient();
                }
                bool removeResult = redis.Remove(key);
                if (isClose)
                {
                    Close(redis);
                }
                return removeResult;
            }
            catch (Exception ex)
            {
                logger.Error("RemoveObject:" + key, ex);
                return false;
            }
        }
        /// <summary>
        /// 移除指定ID的对象
        /// </summary>
        /// <param name="keys"></param>
        public static void RemoveAll(IRedisClient redis = null, bool isClose = true, params string[] keys)
        {
            try
            {
                if (redis == null)
                {
                    redis = GetClient();
                }
                redis.RemoveAll(keys);
                if (isClose)
                {
                    Close(redis);
                }
            }
            catch (Exception ex)
            {
                logger.Error("RemoveObject:" + string.Join(",", keys), ex);
            }
        }
        /// <summary>
        /// 移除指定ID的对象
        /// </summary>
        /// <param name="keys"></param>
        public static void RemoveAll(params string[] keys)
        {
            RemoveAll(null, true, keys);
        }
        /// <summary>
        ///  获取redis客户端对象
        /// </summary>
        public static IRedisClient GetClient()
        {
            try
            {
                return prcm.GetClient();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }
        /// <summary>
        ///  关闭redis
        /// </summary>
        /// <param name="redis"></param>
        public static void Close(IRedisClient redis)
        {
            try
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }



        # endregion


        #region -- 分页缓存 --
        public static T[] GetByPaged<T>(string cacheKey, int startIndex, int endIndex, ref int? count)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                string countStr = redis.GetValueFromHash(cacheKey, "count");
                var countValue = 0;
                if (!string.IsNullOrEmpty(countStr))
                {
                    if (int.TryParse(countStr, out countValue))
                    {
                        count = countValue;
                    }
                }
                string value = redis.GetValueFromHash(cacheKey, startIndex + "-" + endIndex);
                var result = ServiceStack.Text.JsonSerializer.DeserializeFromString<T[]>(value);
                return result;
            }
        }
        public static bool SetByPaged<T>(string cacheKey, int startIndex, int endIndex, T[] items, int count, TimeSpan expired)
        {
            using (IRedisClient redis = prcm.GetClient())
            {
                redis.SetEntryInHash(cacheKey, "count", count.ToString());
                string value = ServiceStack.Text.JsonSerializer.SerializeToString(items);
                redis.ExpireEntryIn(cacheKey, expired);
                return redis.SetEntryInHash(cacheKey, startIndex + "-" + endIndex, value);
            }
        }
        #endregion

    } 
}
