using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;

namespace YSL.Framework.EFRepository.ContextStorage
{
    public class DbContextFactory<T> where T : DbContext, new()
    {
       public static T GetDbContext() 
       {
           IDbContextStorageContainer<T> container = DbContextStorageFactory<T>.CreateStorageContainer();
           var dbContext = container.GetDbContext();
           if (dbContext == null)
           {
               dbContext = new T();
               container.Store(dbContext);
           }
           return dbContext as T;
       }
       public static void ClearDbContext()
       {
           IDbContextStorageContainer<T> container = DbContextStorageFactory<T>.CreateStorageContainer();
           container.Clear();
       }
    }
}
