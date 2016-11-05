using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Common.Log;

namespace YSL.Framework.Cache.Redis
{   
    public sealed class RedisConfigInfoSection : ConfigurationSection
    {
        private static NLog.Logger logger = LogBuilder.NLogger;
        public static RedisConfigInfoSection GetConfig()
        {
            RedisConfigInfoSection section = null;
            try
            {
                section = (RedisConfigInfoSection)ConfigurationManager.GetSection("RedisConfig");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw new ConfigurationErrorsException("Section RedisConfig is error.");
            }

            if (section == null)
            {
                logger.Error("Section RedisConfig is not found.");
                throw new ConfigurationErrorsException("Section RedisConfig is not found.");
            }

            return section;
        }

        public static RedisConfigInfoSection GetConfig(string sectionName)
        {
            RedisConfigInfoSection section = null;
            try
            {
                section = (RedisConfigInfoSection)ConfigurationManager.GetSection(sectionName);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                throw new ConfigurationErrorsException("Section " + sectionName + " is error.");
            }

            if (section == null)
            {
                logger.Error("Section " + sectionName + " is not found.");
                throw new ConfigurationErrorsException("Section " + sectionName + " is not found.");
            }

            return section;
        }

        /// <summary>
        /// 可写的Redis链接地址
        /// </summary>
        [ConfigurationProperty("WriteServerList", IsRequired = true)]
        public string WriteServerList
        {
            get
            {
                return (string)base["WriteServerList"];
            }
            set
            {
                base["WriteServerList"] = value;
            }
        }

        /// <summary>
        /// 可读的Redis链接地址
        /// </summary>
        [ConfigurationProperty("ReadServerList", IsRequired = true)]
        public string ReadServerList
        {
            get
            {
                return (string)base["ReadServerList"];
            }
            set
            {
                base["ReadServerList"] = value;
            }
        }

        /// <summary>
        /// 最大写链接数
        /// </summary>
        [ConfigurationProperty("MaxWritePoolSize", IsRequired = false, DefaultValue = 100)]
        public int MaxWritePoolSize
        {
            get
            {
                int _maxWritePoolSize = (int)base["MaxWritePoolSize"];
                return _maxWritePoolSize > 0 ? _maxWritePoolSize : 100;
            }
            set
            {
                base["MaxWritePoolSize"] = value;
            }
        }

        /// <summary>
        /// 最大读链接数
        /// </summary>
        [ConfigurationProperty("MaxReadPoolSize", IsRequired = false, DefaultValue = 100)]
        public int MaxReadPoolSize
        {
            get
            {
                int _maxReadPoolSize = (int)base["MaxReadPoolSize"];
                return _maxReadPoolSize > 0 ? _maxReadPoolSize : 100;
            }
            set
            {
                base["MaxReadPoolSize"] = value;
            }
        }

        /// <summary>
        /// 自动重启
        /// </summary>
        [ConfigurationProperty("AutoStart", IsRequired = false, DefaultValue = true)]
        public bool AutoStart
        {
            get
            {
                return (bool)base["AutoStart"];
            }
            set
            {
                base["AutoStart"] = value;
            }
        }

        /// <summary>
        /// 本地缓存到期时间，单位:秒
        /// </summary>
        [ConfigurationProperty("LocalCacheTime", IsRequired = false, DefaultValue = 36000)]
        public int LocalCacheTime
        {
            get
            {
                return (int)base["LocalCacheTime"];
            }
            set
            {
                base["LocalCacheTime"] = value;
            }
        }

        /// <summary>
        /// PoolTimeout，单位:秒
        /// </summary>
        [ConfigurationProperty("PoolTimeout", IsRequired = false, DefaultValue = 5)]
        public int PoolTimeout
        {
            get
            {
                return (int)base["PoolTimeout"];
            }
            set
            {
                base["PoolTimeout"] = value;
            }
        }

        /// <summary>
        /// 密码
        /// </summary>
        [ConfigurationProperty("Password", IsRequired = false, DefaultValue = null)]
        public string Password
        {
            get
            {
                return (string)base["Password"];
            }
            set
            {
                base["Password"] = value;
            }
        }

        /// <summary>
        /// DB
        /// </summary>
        [ConfigurationProperty("Db", IsRequired = false, DefaultValue = 0)]
        public int Db
        {
            get
            {
                return (int)base["Db"];
            }
            set
            {
                base["Db"] = value;
            }
        }
    }
}
