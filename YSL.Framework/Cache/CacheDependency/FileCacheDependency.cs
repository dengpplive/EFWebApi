using System;
using System.Web.Caching;

namespace YSL.Framework.Cache
{
    /// <summary>
    /// 缓存依赖的文件
    /// </summary>
    public class FileCacheDependency : ICacheDependency
    {
        private System.Web.Caching.CacheDependency _Dependency = null;

        public FileCacheDependency(String fileName)
        {
            _Dependency = new CacheDependency(fileName);
        }

        public FileCacheDependency(String fileName, DateTime start)
        {
            _Dependency = new CacheDependency(fileName, start);
        }

        public CacheDependency GetDependency()
        {
            return _Dependency;
        }
    }
}
