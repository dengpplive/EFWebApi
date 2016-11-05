using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YSL.Framework.Config;
using YSL.Framework.Config.Models;

namespace YSL.Framework.MsgQuene.ActiveMQ
{
    /*
     ActiveMQConfigSection config=ConfigManage.GetActiveMQConfig("ActiveMQ/MQConfig");
     */
    public class ActiveMQHelper
    {
        #region Topic
        private static Dictionary<string, IConnection> subTopic = new Dictionary<string, IConnection>();
        /// <summary>
        /// 发送主题文本消息
        /// </summary>
        /// <param name="config"></param>
        /// <param name="textMsg"></param>
        public static void SendTopicMessage(ActiveMQConfigSection config, string textMsg)
        {            
            if (!string.IsNullOrEmpty(textMsg))
            {
                var factory = new ConnectionFactory(config.ToConnectString());
                //创建连接
                using (var _connection = factory.CreateConnection())
                {
                    using (ISession session = _connection.CreateSession())
                    {
                        //创建一个主题
                        IDestination destination = new ActiveMQTopic(config.TopicName);
                        //创建生产者
                        IMessageProducer producer = session.CreateProducer(destination);
                        //创建一个文本消息
                        var _message = producer.CreateTextMessage(textMsg);
                        //发送消息
                        producer.Send(_message, MsgDeliveryMode.NonPersistent, MsgPriority.Normal, TimeSpan.MinValue);
                    }
                }
            }
        }
        /// <summary>
        /// 订阅主题
        /// </summary>
        /// <param name="config"></param>
        /// <param name="ClientId"></param>
        /// <param name="ac"></param>
        public static void SubscriberTopic(ActiveMQConfigSection config, Action<MessageResult> ac = null)
        {
            if (!subTopic.ContainsKey(config.ClientId))
            {
                var factory = new ConnectionFactory(config.ToConnectString());
                var conn = factory.CreateConnection();
                conn.ClientId = config.ClientId;
                conn.Start();
                var session = conn.CreateSession();
                var topic = new ActiveMQTopic(config.TopicName);
                IMessageConsumer consumer = session.CreateDurableConsumer(topic, config.ClientId, null, false);
                if (ac != null)
                {
                    consumer.Listener += new MessageListener((m) =>
                    {
                        ac(new MessageResult()
                        {
                            Custom = config.ClientId,
                            IMessage = m
                        });
                    });
                }
                subTopic.Add(config.ClientId, conn);
            }
        }
        /// <summary>
        /// 取消主题订阅
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="ac"></param>
        public static void CancelSubscriberTopic(string clientId, Action<MessageResult> ac = null)
        {
            if (subTopic.ContainsKey(clientId))
            {
                var conn = subTopic[clientId];
                //停止并关闭连接
                conn.Stop();
                conn.Close();
                //移除集合
                subTopic.Remove(clientId);
            }
            if (ac != null)
                ac(new MessageResult()
                {
                    Custom = clientId
                });
        }
        #endregion

        #region Quene
        /// <summary>
        /// 添加一条消息到队列中 含有条件key=val
        /// </summary>
        /// <param name="config"></param>
        /// <param name="textMsg"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        public static void SendMessage(ActiveMQConfigSection config, string textMsg, string key, string val)
        {
            if (!string.IsNullOrEmpty(textMsg))
            {
                var factory = new ConnectionFactory(config.ToConnectString());
                //创建连接
                using (var connection = factory.CreateConnection())
                {
                    //创建一个会话
                    using (ISession session = connection.CreateSession())
                    {
                        //通过会话创建生产者，方法里面new出来的是MQ中的Queue
                        IDestination destination = new ActiveMQQueue(config.TopicName);
                        //创建生产者
                        IMessageProducer producer = session.CreateProducer(destination);
                        //创建一个文本消息
                        var message = producer.CreateTextMessage(textMsg);
                        //给这个对象赋实际的消息
                        //message.Text = textMsg;
                        //设置消息对象的属性，这个很重要哦，是Queue的过滤条件，也是P2P消息的唯一指定属性
                        message.Properties.SetString(key, val);
                        //生产者把消息发送出去，几个枚举参数MsgDeliveryMode是否长链，MsgPriority消息优先级别，发送最小单位，
                        //当然还有其他重载
                        //发送消息
                        producer.Send(message, MsgDeliveryMode.NonPersistent, MsgPriority.Normal, TimeSpan.MinValue);
                    }
                }
            }
        }

        /// <summary>
        /// 订阅 或者读取指定队列的消息 含有过滤条件 filter=key+val
        /// </summary>
        /// <param name="config"></param>
        /// <param name="clientId"></param>
        /// <param name="filter"></param>
        /// <param name="ac"></param>
        public static void ListenMessage(ActiveMQConfigSection config, string filter, Action<MessageResult> ac = null)
        {
            var factory = new ConnectionFactory(config.ToConnectString());
            var conn = factory.CreateConnection();
            conn.ClientId = config.ClientId;
            conn.Start();
            var session = conn.CreateSession();
            var queue = new ActiveMQQueue(config.TopicName);
            //通过会话创建一个消费者，这里就是Queue这种会话类型的监听参数设置
            IMessageConsumer consumer = session.CreateConsumer(queue, filter);
            if (ac != null)
            {
                consumer.Listener += new MessageListener((m) =>
                {
                    ac(new MessageResult()
                    {
                        Custom = config.ClientId,
                        IMessage = m
                    });
                });
            }
        }

        #endregion
    }
}
