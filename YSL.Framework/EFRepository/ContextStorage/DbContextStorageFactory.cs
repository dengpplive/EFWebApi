using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.Entity;
namespace YSL.Framework.EFRepository.ContextStorage
{
    public class DbContextStorageFactory<T> where T : DbContext, new()
    {
        static IDbContextStorageContainer<T> _dbContainer;
        public static IDbContextStorageContainer<T> CreateStorageContainer()
        {
            if (_dbContainer == null)
            {
                if (HttpContext.Current == null)
                    _dbContainer = new ThreadDbContextStorageContainer<T>();
                else
                    _dbContainer = new HttpDbContextStorageContainer<T>();
            }
            return _dbContainer;
        }
    }
}
