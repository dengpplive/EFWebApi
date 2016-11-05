using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace YSL.Framework.DDD
{
    public interface IRepository<T> : IReadOnlyRepository<T>
        where T : IAggregationRoot
    {
        void Create(T t);
        void Update(T t);
        void Delete(T t);
        int ExecuteCommand(string sql, params object[] parames);
        DbRawSqlQuery SqlQuery(string sql, Type type, params object[] parames);
    }
}
