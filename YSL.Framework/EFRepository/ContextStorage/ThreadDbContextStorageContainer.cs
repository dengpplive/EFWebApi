using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Data.Entity;
namespace YSL.Framework.EFRepository.ContextStorage
{
   public class ThreadDbContextStorageContainer<T>:IDbContextStorageContainer<T> where T:DbContext,new()
    {
        private string _key = typeof(T).Name + "ef_dbcontext";
        public T GetDbContext()
        {
            return CallContext.GetData(_key) as T;
        }
        public void Store(T dbContext)
        {
            CallContext.SetData(_key, dbContext);
        }
        public void Clear()
        {
            CallContext.FreeNamedDataSlot(_key);            
        }
    }
}
