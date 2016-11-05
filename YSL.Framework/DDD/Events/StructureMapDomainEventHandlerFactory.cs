using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace YSL.Framework.DDD.Events
{
    /// <summary>
    /// 领域事件处理工厂
    /// </summary>
    public class StructureMapDomainEventHandlerFactory : IDomainEventHandlerFactory
    {
        public IEnumerable<IDomainEventHandler<T>> GetDomainEventHandlersFor<T>() where T : IDomainEvent
        {
            return ObjectFactory.GetAllInstances<IDomainEventHandler<T>>();
        }
    }
}
