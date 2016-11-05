using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YSL.Framework.DDD.Events
{
    public interface IDomainEventHandlerFactory
    {
        IEnumerable<IDomainEventHandler<T>> GetDomainEventHandlersFor<T>() where T : IDomainEvent;
    }

}
