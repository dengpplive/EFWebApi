using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Framework.DDD
{
    public interface IUnitOfWork : IDependency,IDisposable
    {
        void RegisterAmended(IAggregationRoot entity, IUnitOfWorkRepository unitOfWorkRepository);
        void RegisterNew(IAggregationRoot entity, IUnitOfWorkRepository unitOfWorkRepository);
        void RegisterRemoved(IAggregationRoot entity, IUnitOfWorkRepository unitOfWorkRepository);
        void Commit();
    }
}
