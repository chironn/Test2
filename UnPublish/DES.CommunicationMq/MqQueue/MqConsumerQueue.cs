using DES.CommunicationMq.Entities;
using DES.Core.Interfaces;
using RabbitMQ.Client;
using System;
using RabbitMQ.Client.Events;
/* ==============================================================================
* 类型名称：MqPopQueue  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/19 15:38:50
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

namespace DES.CommunicationMq.MqQueue
{
    /// <summary>
    /// 
    /// </summary>
    public class MqConsumerQueue : IPopQueue
    {
        #region IPopQueue
        public Action<ICommunicationEntity> ReceiveCallback { get; set; }
        private MqConfig _mqConfig;
        public IQueueConfig Config { get { return _mqConfig; } set { _mqConfig = (MqConfig)value; } }

        public Action<ICommunicationQueue, Exception, string> ExcptionCallback { get; set; }

        public string FindKey { get { return Config == null || Config.FindKey == null ? string.Empty : Config.FindKey; } }

        public void Initialize()
        {
            try
            {
                //初始化连接
                if (_readConnection == null)
                {
                    _readConnection = _factory.Connection(_mqConfig);
                }

                //初始化IMode
                if (_readChannel == null)
                {
                    _readChannel = _readConnection.CreateModel();
                    _readChannel.BasicQos(0, _prefetchCount, false);
                    try
                    {
                        // 申明交换
                        _readChannel.ExchangeDeclare(_mqConfig.ExchangeName, _mqConfig.ExchangeType, _mqConfig.ExchangeDurable);

                    }
                    catch (Exception ex)
                    {
                        if (ExcptionCallback != null)
                            ExcptionCallback(this, ex, string.Format("mq队列声明失败!"));
                    }
                    // 声明队列
                    _readChannel.QueueDeclare(_mqConfig.QueueName, true, false, false, null);

                    // 绑定交换
                    _readChannel.QueueBind(_mqConfig.QueueName, _mqConfig.ExchangeName, _mqConfig.BindKey);

                }



                //初始化Consumer
                if (_consumer == null)
                {
                    _consumer = new QueueingBasicConsumer(_readChannel);

                    _readChannel.BasicConsume(_mqConfig.QueueName, _noAck, _consumer);
                }

                if (_mainCancelSource != null) return;

                _mainCancelSource = new System.Threading.CancellationTokenSource();
                System.Threading.Tasks.Task.Factory.StartNew(ReceiveThread, _mainCancelSource.Token);
            }
            catch (Exception ex)
            {
                if (ExcptionCallback != null)
                {
                    ExcptionCallback(this, ex, string.Format("mq队列初始化失败!"));
                }
                ClearChannel();
            }
        }
        #endregion

        #region 私有成员
        private readonly MqFactory _factory;

        // mq 连接
        private IConnection _readConnection;

        // mq 通道
        private IModel _readChannel;

        // mq 队列消费者
        private QueueingBasicConsumer _consumer;

        // 预取消息调数
        private const ushort _prefetchCount = 100;

        // 是否用发送ack
        private const bool _noAck = true;

        // 主进程取消等待信号
        private System.Threading.CancellationTokenSource _mainCancelSource;


        /// <summary>
        /// 当前队列是否暂停
        /// </summary>
        public bool IsSuspend { get; private set; }

        // 主线程阻塞信号
        private readonly System.Threading.ManualResetEvent _blockWait = new System.Threading.ManualResetEvent(false);

        // 读取超时等待时间，单位:毫秒
        private const int _readTimeOut = 300;

        #endregion

        #region 数据接收线程

        private void ReceiveThread()
        {
            var errorTimes = 0;
            while (!_mainCancelSource.IsCancellationRequested)
            {
                try
                {
                    Initialize();
                    Receive();
                    errorTimes = 0;
                    // 判断线程是否暂停
                    if (!IsSuspend) continue;
                    _blockWait.Reset();
                    // 无限等待
                    _blockWait.WaitOne(System.Threading.Timeout.Infinite);
                }
                catch (Exception ex)
                {
                    errorTimes++;
                    if (ExcptionCallback != null)
                    {
                        ExcptionCallback(this, ex,
                            string.Format("MQ 数据接收线程异常,异常次数:{0},超过3次则自动重连", errorTimes));
                    }
                }
                // 异常超过3次，将清空连接，重新连接
                if (errorTimes > 3)
                {
                    ClearConnection();
                }
            }
        }

        private void Receive()
        {
            try
            {
                BasicDeliverEventArgs basicDeliverEventArgs;
                if (!_consumer.Queue.Dequeue(_readTimeOut, out basicDeliverEventArgs)) return;

                if (ReceiveCallback == null) return;


                var entity = _factory.GetOrCreateEntity(basicDeliverEventArgs.Body, basicDeliverEventArgs.Body.Length);

                entity.FindKey = FindKey;


                try
                {
                    ReceiveCallback(entity);
                }
                catch (Exception ex)
                {
                    if (ExcptionCallback != null)
                    {
                        ExcptionCallback(this, ex, string.Format("mq队列数据接收回调，未捕获异常"));
                    }
                }

                //if (!_noAck)
                //{
                //    _readChannel.BasicAck(basicDeliverEventArgs.DeliveryTag, false);
                //}
            }
            catch (Exception ex)
            {
                if (ExcptionCallback != null)
                {
                    ExcptionCallback(this, ex, string.Format("mq队列数据接收失败!"));
                }
                ClearChannel();
            }
        }

        #endregion

        #region 构造函数

        public MqConsumerQueue(MqFactory factory)
        {
            _factory = factory;
        }
        #endregion

        #region 异常清理
        /// <summary>
        /// 清除/关闭Channel
        /// </summary>
        public void ClearChannel()
        {
            try
            {
                //释放Consumer
                //释放IMode
                if (_readChannel != null)
                {
                    _readChannel.Close();
                    _readChannel.Dispose();
                }

                _consumer = null;

                _readChannel = null;

            }
            catch (Exception ex)
            {
                if (ExcptionCallback != null)
                {
                    ExcptionCallback(this, ex, string.Format("mq队列释放失败!"));
                }
            }
        }
        /// <summary>
        /// 强制清空连接
        /// </summary>
        public void ClearConnection()
        {
            if (_readConnection == null) return;
            try
            {
                _readConnection.Close();
                _readConnection.Dispose();
            }
            catch (Exception ex)// 强制清空连接
            {
                if (ExcptionCallback != null)
                {
                    ExcptionCallback(this, ex, string.Format("mq连接释放失败!"));
                }
            }
            finally
            {
                _readConnection = null;
            }
        }
        #endregion

        #region 暂停接收数据


        public void Suspend()
        {
            IsSuspend = true;
        }
        public void Restart()
        {
            Initialize();
            IsSuspend = false;
            _blockWait.Set();
        }
        #endregion


        public void Dispose()
        {
            if (_mainCancelSource == null) return;

            _mainCancelSource.Cancel();
            _mainCancelSource.Dispose();
            ClearChannel();
            ClearConnection();
            _mainCancelSource = null;
        }
    }
}
