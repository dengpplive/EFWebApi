using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Framework.EFRepository.ContextStorage;
using YSL.Framework.DDD;
using YSL.Common.Log;
namespace YSL.Framework.EFRepository.UnitOfWork
{
    /// <summary>
    /// 非查询类的操作 UnitOfWork
    /// </summary>
    /// <typeparam name="T">DB上下文</typeparam>
    public class EFUnitOfWork<T> : IUnitOfWork, IDisposable
        where T : DbContext, new()
    {

        public void RegisterAmended(IAggregationRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            unitOfWorkRepository.PersistUpdateOf(entity);
        }

        public void RegisterNew(IAggregationRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            unitOfWorkRepository.PersistCreationOf(entity);
        }

        public void RegisterRemoved(IAggregationRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            unitOfWorkRepository.PersistDeletionOf(entity);
        }

        public void Commit()
        {
            try
            {
                var result = DbContextFactory<T>.GetDbContext().GetValidationErrors();
                foreach (var item in result)
                {
                    foreach (var itemA in item.ValidationErrors)
                    {
                        LogBuilder.NLogger.Error(string.Format("{0}:{1}", itemA.PropertyName, itemA.ErrorMessage));
                    }
                }
                int status = DbContextFactory<T>.GetDbContext().SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                LogBuilder.NLogger.Error(string.Format("Commit:{0}", e.Message));
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
            DbContextFactory<T>.GetDbContext().Dispose();
            DbContextFactory<T>.ClearDbContext();
            GC.SuppressFinalize(this);
        }
    }
}
