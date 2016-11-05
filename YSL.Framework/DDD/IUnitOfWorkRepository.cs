using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace YSL.Framework.DDD
{
    public interface IUnitOfWorkRepository : IDependency
    {
       void PersistCreationOf(IAggregationRoot entity);
       void PersistUpdateOf(IAggregationRoot entity);
       void PersistDeletionOf(IAggregationRoot entity);
       int ExecuteCommand(string sql,params object[] parames);
       DbRawSqlQuery SqlQuery(string sql, Type type, params object[] parames);
    }
}
