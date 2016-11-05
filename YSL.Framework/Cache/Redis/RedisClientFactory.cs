using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Common.Log;

namespace YSL.Framework.Cache.Redis
{
    /// <summary>
    /// 缓存客户端管理器工厂
    /// </summary>
    public class RedisClientFactory
    {
        private static NLog.Logger logger = LogBuilder.NLogger;
        private static RedisConfigInfoSection redisConfigInfo = RedisConfigInfoSection.GetConfig();
        private static IRedisClient client = null;
        private static object lockObject = new object();
        private static PooledRedisClientManager PRCM;
        private static string redisIp;
        private static int redisPort;
        static RedisClientFactory()
        {
            string[] writeServerList = SplitString(redisConfigInfo.WriteServerList, ",");
            string[] readServerList = SplitString(redisConfigInfo.ReadServerList, ",");
            PRCM = CreateManager(writeServerList, readServerList, redisConfigInfo.Db);
            redisIp = writeServerList[0].Substring(0, writeServerList[0].IndexOf(':'));
            redisPort = Convert.ToInt32(writeServerList[0].Substring(writeServerList[0].IndexOf(':') + 1));
        }

        private static string[] SplitString(string strSource, string split)
        {
            return strSource.Split(split.ToArray());
        }
        private static PooledRedisClientManager Manager = null;
        public static PooledRedisClientManager CreateManager(string[] readWriteHosts, string[] readOnlyHosts, int initialDB = 0)
        {
            if (Manager == null)
            {
                Manager = new PooledRedisClientManager(readWriteHosts, readOnlyHosts, new RedisClientManagerConfig()
                {
                    MaxWritePoolSize = redisConfigInfo.MaxWritePoolSize,
                    MaxReadPoolSize = redisConfigInfo.MaxReadPoolSize,
                    AutoStart = redisConfigInfo.AutoStart
                }, initialDB, 50, redisConfigInfo.PoolTimeout);
            }
            return Manager;
        }
      
        /// <summary>
        /// 获取一个redis客户端接口对象
        /// </summary>
        /// <returns></returns>
        public static IRedisClient GetIRedisInstance()
        {
            try
            {
                if (client == null)
                {
                    lock (lockObject)
                    {
                        if (client == null)
                        {
                            client = PRCM.GetClient();
                        }
                    }
                }
                return client;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }

        public static RedisNativeClient GetRedisNativeClient()
        {
            try
            {
                if (string.IsNullOrEmpty(redisConfigInfo.Password))
                {
                    RedisNativeClient client = new RedisNativeClient(redisIp, redisPort, null, redisConfigInfo.Db);
                    return client;
                }
                else
                {
                    RedisNativeClient client = new RedisNativeClient(redisIp, redisPort, redisConfigInfo.Password, redisConfigInfo.Db);
                    return client;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }
    }
}
