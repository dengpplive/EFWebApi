using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Framework.DDD
{
    /// <summary>
    /// 专用于查询操作的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyRepository<T> : IBaseRepository<T>
        where T : IAggregationRoot
    {
        IQueryable<T> FindAll(Expression<Func<T, bool>> exp);
        IQueryable<T> FindAll();
        /// <summary>
        /// 无状态查询
        /// </summary>
        /// <returns></returns>
        IQueryable<T> FindAllNoTracking();

        /// <summary>
        /// 无状态查询带有表达式的查询
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        IQueryable<T> FindAllNoTracking(Expression<Func<T, bool>> exp);

        /// <summary>
        /// 无状态分页查询
        /// </summary>
        /// <typeparam name="S"></typeparam>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="total"></param>
        /// <param name="whereLambda"></param>
        /// <param name="isAsc"></param>
        /// <param name="orderByLambda"></param>
        /// <returns></returns>
        IQueryable<T> FindPagerEntities<S>(int pageSize, int pageIndex, out int total,
            Func<T, bool> whereLambda, bool isAsc, Func<T, S> orderByLambda);

    }
    public interface IBaseRepository<T> : IDependency
        where T : IAggregationRoot
    {
    }
}
