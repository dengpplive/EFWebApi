using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
namespace YSL.Framework.EFRepository.ContextStorage
{
   public interface IDbContextStorageContainer<T> where T:DbContext,new()
    {
       T GetDbContext();
       void Store(T dbContext);
       void Clear();
    }
}
