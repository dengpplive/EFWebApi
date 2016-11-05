using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Framework.DDD
{
    /// <summary>
    /// 泛型模型领域根
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAggregationRoot<T> : IAggregationRoot
        where T : struct
    {
        T Id { get; set; }
    }
    /// <summary>
    /// 模型领域根
    /// </summary>
    public interface IAggregationRoot
    {
    }
}
