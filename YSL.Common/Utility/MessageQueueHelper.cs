using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace YSL.Common.Utility
{
    /// <summary>
    /// MessageQueue helper class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MessageQueueHelper<T> : MarshalByRefObject where T : class, new()
    {
        public MessageQueueHelper(string path)
        {
            m_AllowException = true;
            if (MessageQueue.Exists(path))
                m_Msq = new MessageQueue(path);
            else
            {
                m_Msq = MessageQueue.Create(path);
                m_Msq.MaximumQueueSize = 1000;// CommonSettings.QueueMaxSize;
            }
            m_Msq.SetPermissions("Everyone", System.Messaging.MessageQueueAccessRights.FullControl);

            m_Msq.Formatter = new XmlMessageFormatter(new Type[] { typeof(T) });
            m_Msq.Label = typeof(T).Name;
            m_Msq.ReceiveCompleted += new ReceiveCompletedEventHandler(Msq_ReceiveCompleted);
        }
        ~MessageQueueHelper()
        {
            Close();
        }
        private MessageQueue m_Msq;
        private bool m_AllowException;

        public bool AllowException
        {
            get { return m_AllowException; }
            set { m_AllowException = value; }
        }
        private bool MssageQueueReady()
        {
            if (m_Msq == null)
                if (AllowException)
                    throw new Exception("The message queue is not ready.");
                else
                    return false;
            else
                return true;
        }
        public void Send(object msg)
        {
            if (!msg.GetType().Equals(typeof(T))) return;
            if (!MssageQueueReady()) return;
            try
            {
                m_Msq.Send(msg);
            }
            catch
            {
                // TODO: Send Message queue ;failed
            }
        }
        public void Send(object msg, string label)
        {
            if (!MssageQueueReady()) return;
            try
            {
                m_Msq.Send(msg, label);
            }
            catch
            {
                // TODO: Send Message queue ;failed
            }
        }
        public T Receive()
        {
            if (!MssageQueueReady()) return default(T);
            Message m = m_Msq.Receive(MessageQueueTransactionType.Single);
            return m.Body as T;
        }
        public IList<T> ReceiveAll()
        {
            if (!MssageQueueReady()) return null;
            Message[] ms = m_Msq.GetAllMessages();
            IList<T> list = new List<T>();
            foreach (Message m in ms)
            {
                list.Add(m.Body as T);
            }
            return list;
        }
        public T Peek()
        {
            if (!MssageQueueReady()) return default(T);
            //m_Msq.Formatter = new XmlMessageFormatter(new Type[] { ;typeof(T) });
            Message m = m_Msq.Peek();
            return m.Body as T;
        }
        public void AsynchronismReceive()
        {
            if (!MssageQueueReady()) return;
            m_Msq.BeginReceive();
        }
        public void EndReceive()
        {
            if (!MssageQueueReady()) return;
        }
        private void Msq_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            MessageQueue mq = (MessageQueue)sender;
            Message m = mq.EndReceive(e.AsyncResult);
            if (ReceiveEvent != null)
                ReceiveEvent(this, new ReceiveEventArgs<T>(m.Body as T));
            mq.BeginReceive();
        }
        public event ReceiveEventHandler ReceiveEvent;
        public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs<T> e);
        public void Close()
        {
            if (m_Msq != null)
                m_Msq.Close();
        }
    }

    public class CommonSettings
    {
        public static int QueueMaxSize { get; set; }
    }

    public class ReceiveEventArgs<T> : EventArgs where T : class
    {
        public ReceiveEventArgs(T result)
        {
            m_Result = result;
        }
        private T m_Result;

        public T Result
        {
            get { return m_Result; }
            //set { m_Result = value; }
        }
    }
}