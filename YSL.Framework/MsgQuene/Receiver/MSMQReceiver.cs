using System;
using System.Configuration;
using System.Messaging;

namespace YSL.Framework.MsgQuene.Receiver
{
    public class MSMQReceiver : CommandReceiver
    {
        private MessageQueue _queue = null;
        private string _path = string.Empty;

        public MSMQReceiver(string path)
        {
            this._path = path;
        }

        public override void Start()
        {
            if (_queue == null)
            {
                try
                {
                    _queue = MessageQueue.Exists(this._path)
                        ? new MessageQueue(this._path, QueueAccessMode.Receive)
                        : MessageQueue.Create(this._path);
                    _queue.Formatter = new XmlMessageFormatter(new[] { typeof(string) });
                    _queue.ReceiveCompleted += (object sender, ReceiveCompletedEventArgs e) =>
                    {
                        var mq = ((MessageQueue)sender).EndReceive(e.AsyncResult);
                        try
                        {
                            OnCommandReceived(e.Message.Label, mq);
                        }
                        catch (Exception ex)
                        {
                            //...
                        }
                        finally
                        {
                            if (_queue != null)
                                _queue.BeginReceive();
                        }
                    };
                    _queue.BeginReceive();
                }
                catch (Exception ex)
                {
                    //..
                }
            }
        }
        public override void Dispose()
        {
            if (_queue != null)
            {
                _queue.Dispose();
                _queue = null;
            }
        }
    }

    public abstract class BaseReceiver : IDisposable
    {
        public abstract void Start();

        public abstract void Dispose();
    }
}