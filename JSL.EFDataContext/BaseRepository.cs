using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YSL.Framework.DDD;
using YSL.Framework.EFRepository.ContextStorage;
using YSL.Framework.EFRepository.UnitOfWork;
namespace JSL.EFDataContext
{
    public abstract class BaseRepository<T> : IRepository<T>
         where T : class, IAggregationRoot
    {
        private IUnitOfWork _uow;
        private IUnitOfWorkRepository _uowr;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="uowr"></param>
        public BaseRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
        {
            this._uow = uow;
            this._uowr = uowr;
        }
        public virtual IQueryable<T> FindAll(Expression<Func<T, bool>> exp)
        {
            return DbContextFactory<XCY_DataContext>.GetDbContext().Set<T>().Where(exp);
        }
        /// <summary>
        /// 无状态查询
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        public virtual IQueryable<T> FindAllNoTracking(Expression<Func<T, bool>> exp)
        {
            return DbContextFactory<XCY_DataContext>.GetDbContext().Set<T>().AsNoTracking().Where(exp);
        }
        public virtual IQueryable<T> FindAll()
        {
            return DbContextFactory<XCY_DataContext>.GetDbContext().Set<T>().AsQueryable();
        }
        /// <summary>
        /// 无状态查询
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> FindAllNoTracking()
        {
            return DbContextFactory<XCY_DataContext>.GetDbContext().Set<T>().AsNoTracking().AsQueryable();
        }
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
        public virtual IQueryable<T> FindPagerEntities<S>(int pageSize, int pageIndex, out int total,
            Func<T, bool> whereLambda, bool isAsc, Func<T, S> orderByLambda)
        {
            var db = DbContextFactory<XCY_DataContext>.GetDbContext();
            var tempData = db.Set<T>().AsNoTracking().Where<T>(whereLambda);
            //总条数
            total = tempData.Count();
            //排序 升序
            if (isAsc)
            {
                tempData = tempData.OrderBy<T, S>(orderByLambda).
                      Skip<T>(pageSize * (pageIndex - 1)).
                      Take<T>(pageSize);
            }
            else
            {
                tempData = tempData.OrderByDescending<T, S>(orderByLambda).
                     Skip<T>(pageSize * (pageIndex - 1)).
                     Take<T>(pageSize);
            }
            return tempData.AsQueryable();
        }
        public virtual void Create(T t)
        {
            _uow.RegisterNew(t, _uowr);
        }
        public virtual void Update(T t)
        {
            _uow.RegisterAmended(t, _uowr);
        }
        public virtual void Delete(T t)
        {
            _uow.RegisterRemoved(t, _uowr);
        }
        public virtual int ExecuteCommand(string sql, params object[] parames)
        {
            var db = DbContextFactory<XCY_DataContext>.GetDbContext();
            return db.Database.ExecuteSqlCommand(sql, parames);
        }

        public virtual DbRawSqlQuery SqlQuery(string sql, Type type, params object[] parames)
        {
            var db = DbContextFactory<XCY_DataContext>.GetDbContext();
            return db.Database.SqlQuery(type, sql, parames);
        }
        public XCY_DataContext DbContext
        {
            get
            {
                return DbContextFactory<XCY_DataContext>.GetDbContext();
            }
        }

    }
}
