/* ==============================================================================
* 类型名称：MqProductorQueue  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/28 9:54:16
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using DES.CommunicationMq.Entities;
using DES.Core.Interfaces;
using RabbitMQ.Client;
using DES.Core;

namespace DES.CommunicationMq.MqQueue
{
    /// <summary>
    /// 
    /// </summary>
    public class MqProductorQueue : IPushQueue
    {
        #region IPushQueue
        private MqConfig _mqConfig;

        public IQueueConfig Config { get { return _mqConfig; } set { _mqConfig = (MqConfig)value; } }

        public Action<ICommunicationQueue, Exception, string> ExcptionCallback { get; set; }

        public string FindKey { get { return Config == null || Config.FindKey == null ? string.Empty : Config.FindKey; } }

        public void Initialize()
        {
            //初始化连接
            if (_readConnection == null)
            {
                _readConnection = _factory.Connection(_mqConfig);
            }

            //初始化IMode
            if (_writeChannel == null)
            {
                _writeChannel = _readConnection.CreateModel();
            }

            if (_properties != null) return;

            _properties = _writeChannel.CreateBasicProperties();
            _properties.SetPersistent(true);
        }

        private int errorTimes;

        public void Push(ICommunicationEntity entity)
        {

            var baseEntity = entity as BufferEntity;
            if (baseEntity == null) return;

            var mqEntity = (MqComunicationEntity)entity;

            var exchangeName = mqEntity.ExchangeName;
            var routingKey = mqEntity.RouteingKey;


            exchangeName = string.IsNullOrEmpty(exchangeName) ? _mqConfig.ExchangeName : exchangeName;
            routingKey = string.IsNullOrEmpty(routingKey)
                             ? string.IsNullOrEmpty(_mqConfig.RouteingKey) ? _mqConfig.BindKey : _mqConfig.RouteingKey
                             : routingKey;

            try
            {
                Initialize();
                var bytes = new byte[baseEntity.BytesLength];
                Array.Copy(baseEntity.Bytes, 0, bytes, 0, bytes.Length);
                // 发布消息
                _writeChannel.BasicPublish(exchangeName, routingKey, _properties, bytes);
            }
            catch (Exception ex)
            {
                if (ExcptionCallback != null)
                {
                    ExcptionCallback(this, ex, string.Format("mq 发布消息失败!"));
                }
                ClearChannel();
                errorTimes++;
                // 错误次数超过3次，重新创建连接
                if (errorTimes > 3)
                {
                    ClearConnection();
                }
            }
            finally
            {
                _factory.GivebackEntity(baseEntity);
            }

        }

        
        #endregion

        #region 私有成员
        // mq 连接
        private IConnection _readConnection;

        // mq 通道
        private IModel _writeChannel;

        // mq工厂
        private readonly MqFactory _factory;

        // mq属性
        private IBasicProperties _properties;

        #endregion

        #region 构造函数

        public MqProductorQueue(MqFactory factory)
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
            if (_writeChannel == null) return;

            try
            {
                //释放IMode
                _writeChannel.Close();
                _writeChannel.Dispose();
            }
            catch (Exception ex)
            {
                if (ExcptionCallback != null)
                {
                    ExcptionCallback(this, ex, string.Format("mq队列释放失败!"));
                }
            }
            finally
            {
                _writeChannel = null;
                _properties = null;
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

        public void Dispose()
        {
            ClearChannel();
            ClearConnection();
        }
    }
}
