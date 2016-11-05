using System;
using System.Web.Caching;
using System.Configuration;

namespace YSL.Framework.Cache
{
    /// <summary>
    /// 数据库表的缓存依赖
    /// </summary>
    public class TableCacheDependency : ICacheDependency
    {
        //数据库缓存依赖数据库名称
        private static readonly string _DataBaseName = "";// ConfigurationManager.AppSettings["CacheDatabaseName"];
        //分隔符
        private char[] configurationSeparator = new char[] { ',' };
        private AggregateCacheDependency _Dependency = new AggregateCacheDependency();

        public TableCacheDependency(string tableConfig)
        {
            string[] tables = tableConfig.Split(configurationSeparator);
            foreach (string table in tables)
            {
                _Dependency.Add(new SqlCacheDependency(_DataBaseName, table));
            }
        }
        public System.Web.Caching.CacheDependency GetDependency()
        {
            return _Dependency;
        }
    }
}
