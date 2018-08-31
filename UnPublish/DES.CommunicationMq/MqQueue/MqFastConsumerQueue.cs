/* ==============================================================================
* 类型名称：MqFastConsumerQueue  
* 类型描述：快速响应队列，该队列不进行队列缓存，通过同步接收数据
* 创 建 者：linfk
* 创建日期：2018/1/12 11:18:53
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using DES.CommunicationMq.Entities;
using DES.Core.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Framing.Impl;

namespace DES.CommunicationMq.MqQueue
{
    /// <summary>
    /// 快速响应队列，该队列不进行队列缓存，通过同步接收数据
    /// </summary>
    public class MqFastConsumerQueue : IPopQueue
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
                    _readConnection = (AutorecoveringConnection)_factory.Connection(_mqConfig);
                    _readConnection.Recovery += (_, recoverable) =>
                        {
                            if (ExcptionCallback != null)
                            {
                                ExcptionCallback(this, new Exception("连接恢复"), "通信连接恢复");
                            }
                        };
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
                    catch(Exception ex)
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
                if (_consumer != null) return;

                _consumer = new FastConsumer(_readChannel);

                _consumer.BasicDeliverEvnet += _consumer_BasicDeliverEvnet;

                _readChannel.BasicConsume(_mqConfig.QueueName, _noAck, _consumer);
            }
            catch (Exception ex)
            {
                if (ExcptionCallback != null)
                {
                    ExcptionCallback(this, ex, string.Format("mq队列初始化失败!"));
                }
            }
        }



        private void _consumer_BasicDeliverEvnet(object sender, BasicDeliverEventArgs basicDeliverEventArgs)
        {
            if (ReceiveCallback == null) return;
            try
            {

                var entity = _factory.GetOrCreateEntity(basicDeliverEventArgs.Body, basicDeliverEventArgs.Body.Length);
                entity.FindKey = FindKey;
                ReceiveCallback(entity);
            }
            catch (Exception ex)
            {
                if (ExcptionCallback != null)
                {
                    ExcptionCallback(this, ex, string.Format("mq队列数据接收回调，未捕获异常"));
                }
            }
        }
        #endregion

        #region 私有成员
        private readonly MqFactory _factory;

        // mq 连接
        private AutorecoveringConnection _readConnection;

        // mq 通道
        private IModel _readChannel;

        // mq 队列消费者
        private FastConsumer _consumer;

        // 预取消息调数
        private const ushort _prefetchCount = 100;

        // 是否用发送ack
        private const bool _noAck = true;


        /// <summary>
        /// 当前队列是否暂停
        /// </summary>
        public bool IsSuspend { get; private set; }

        #endregion


        #region 构造函数

        public MqFastConsumerQueue(MqFactory factory)
        {
            _factory = factory;
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
        }
        #endregion


        public void Dispose()
        {

        }
    }
}
