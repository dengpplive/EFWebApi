using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YSL.Common.Extender;
using YSL.Common.Log;

namespace YSL.Framework.DDD.Events
{
    /*
      1.写一个数据类T 继承自 IDomainEvent
      2.写一个操作类 继承自 IDomainEventHandler<T> 实现Handle方法即可      
      2.使用的地方调用 DomainEvents.Raise<T>(T domainEvent) 就会调用相应的Handle方法
     */
    /// <summary>
    /// 领域事件
    /// </summary>
    public static class DomainEvents
    {
        public static IDomainEventHandlerFactory DomainEventHandlerFactory
        {
            get
            {
                return new StructureMapDomainEventHandlerFactory();
            }
        }
        /// <summary>
        /// 触发领域事件
        /// </summary>
        /// <typeparam name="T">领域数据实体类型</typeparam>
        /// <param name="domainEvent">领域数据</param>
        public static void Raise<T>(T domainEvent) where T : IDomainEvent
        {
            try
            {
                DomainEventHandlerFactory
                    .GetDomainEventHandlersFor<T>()
                    .ForEach(h =>
                    {
                        h.Handle(domainEvent);
                    });
            }
            catch (Exception e)
            {
                //Console.WriteLine("时间错误:" + e.Message);
                LogBuilder.NLogger.Error(string.Format("Raise:{0}", e.Message));
            }
        }
    }
}
