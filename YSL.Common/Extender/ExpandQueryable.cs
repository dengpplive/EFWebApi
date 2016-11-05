using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Extender
{
    /// <summary>
    /// EF扩展工具类
    /// </summary>
    public static class ExpandQueryable
    {
        private static readonly ConcurrentDictionary<string, LambdaExpression> Cache = new ConcurrentDictionary<string, LambdaExpression>();

        /// <summary>
        /// 升序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetLambdaExpression(propertyName, typeof(T));
            return Queryable.OrderBy(source, keySelector);
        }
        /// <summary>
        /// 降序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetLambdaExpression(propertyName, typeof(T));
            return Queryable.OrderByDescending(source, keySelector);
        }
        /// <summary>
        /// 次要的升序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetLambdaExpression(propertyName, typeof(T));

            return Queryable.ThenBy(source, keySelector);

        }
        /// <summary>
        /// 次要的降序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string propertyName)
        {
            dynamic keySelector = GetLambdaExpression(propertyName, typeof(T));
            return Queryable.ThenByDescending(source, keySelector);
        }
        private static LambdaExpression GetLambdaExpression(string propertyName, Type type)
        {
            string key = type.FullName + "_" + propertyName;
            if (Cache.ContainsKey(key))
            {
                return Cache[key];
            }
            ParameterExpression param = Expression.Parameter(type);
            MemberExpression body = Expression.Property(param, propertyName);
            LambdaExpression keySelector = Expression.Lambda(body, param);
            Cache[key] = keySelector;
            return keySelector;
        }

        /// <summary>
        /// Or关键字
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        //public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
        //                                                   Expression<Func<T, bool>> expr2)
        //{
        //    var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
        //    return Expression.Lambda<Func<T, bool>>
        //          (Expression.Or(expr1.Body, invokedExpr), expr1.Parameters);
        //}
        /// <summary>
        /// 转换到表结构
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DataTable ToDataTableSchema<T>(this IQueryable<T> source) where T : class
        {
            var t = typeof(T);
            var ps = t.GetProperties();
            DataTable dt = new DataTable();
            foreach (var p in ps)
            {
                dt.Columns.Add(new DataColumn(p.Name, p.PropertyType));
            }
            return dt;
        }
        /// <summary>
        /// 转换到DataTable结构和数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IQueryable<T> source) where T : class
        {
            var t = typeof(T);
            var ps = t.GetProperties();
            DataTable dt = new DataTable();
            foreach (var p in ps)
            {
                dt.Columns.Add(new DataColumn(p.Name, p.PropertyType));
            }
            foreach (var item in source)
            {
                var row = dt.NewRow();
                foreach (var p in ps)
                {
                    row[p.Name] = p.GetValue(item, null);
                }
                dt.Rows.Add(row);
            }
            return dt;
        }

        /// <summary>
        /// 实现In查询
        /// 例子：string[] names = new string[] { "a", "b" }; 
        /// var users = te.User.Where(ContainsExpression<User, string>(a => a.User_name, names));   
        /// </summary>
        /// <typeparam name="TElement"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="valueSelector"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Expression<Func<TElement, Boolean>> ContainsExpression<TElement, TValue>(Expression<Func<TElement, TValue>> valueSelector, IEnumerable<TValue> values)
        {
            if (valueSelector == null)
            {
                throw new ArgumentNullException("valueSelector");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            ParameterExpression parameterExpression = valueSelector.Parameters.Single();
            if (!values.Any())
            {
                return e => false;
            }
            IEnumerable<Expression> equals = values.Select(value => (Expression)Expression.Equal(valueSelector.Body, Expression.Constant(value, typeof(TValue))));
            Expression body = equals.Aggregate<Expression>((accumulate, equal) => Expression.Or(accumulate, equal));
            return Expression.Lambda<Func<TElement, Boolean>>(body, parameterExpression);
        }

        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge) where T : class
        {

            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);
            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            // apply composition of lambda expression bodies to parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);

        }

        /// <summary>
        /// And关键字
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, Boolean>> And<T>(this Expression<Func<T, Boolean>> first, Expression<Func<T, Boolean>> second) where T : class
        {
            return first.Compose(second, Expression.And);
        }

        /// <summary>
        /// Or关键字
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static Expression<Func<T, Boolean>> Or<T>(this Expression<Func<T, Boolean>> first, Expression<Func<T, Boolean>> second) where T : class
        {
            return first.Compose(second, Expression.Or);
        }
        public class ParameterRebinder : ExpressionVisitor
        {
            private readonly Dictionary<ParameterExpression, ParameterExpression> map;

            public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression replacement;
                if (map.TryGetValue(p, out replacement))
                {
                    p = replacement;
                }
                return base.VisitParameter(p);
            }
        }
    }
}
