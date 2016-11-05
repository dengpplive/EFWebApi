using System;
using System.Web.Caching;

namespace YSL.Framework.Cache
{
    public interface ICacheDependency
    {
        /// <summary>
        /// 在存储于 ASP.NET 应用程序的 System.Web.Caching.Cache 对象中的项与文件、缓存键、文件或缓存键的数组或另一个 System.Web.Caching.CacheDependency
        /// 对象之间建立依附性关系。 System.Web.Caching.CacheDependency 类监视依附性关系，以便在任何这些对象更改时，该缓存项都会自动移除。
        /// </summary>
        /// <returns></returns>
        CacheDependency GetDependency();
    }
}
