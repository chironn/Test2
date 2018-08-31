using System;
using System.Collections.Generic;
using DES.CommunicationMq.Entities;
using DES.Core;
using DES.Core.Interfaces;
using RabbitMQ.Client;
using DES.CommunicationMq.MqQueue;

namespace DES.CommunicationMq
{
    public class MqFactory : CommunicationProvider
    {
        public readonly int MaxCount = 10 * 1024;
        private readonly System.Collections.Concurrent.BlockingCollection<MqComunicationEntity> _entitiesPool =
            new System.Collections.Concurrent.BlockingCollection<MqComunicationEntity>();

        public override List<IQueueConfig> CreateConfig(string connectString)
        {
            XmlMqConfigArray xmlMqConfigArray;
            if (XmlMqConfigArray.TryParse(connectString, out xmlMqConfigArray))
            {
                var result = new List<IQueueConfig>();
                foreach (var item in xmlMqConfigArray.XmlMqConfigs)
                {
                    result.AddRange(item.ToList());
                }
                return result;
            }

            XmlMqConfig config;
            return XmlMqConfig.TryParse(connectString, out config) ? config.ToList() : new List<IQueueConfig>();
        }

        public override ICommunicationQueue CreateQueue(IQueueConfig baseConfig)
        {
            var config = ((MqConfig)baseConfig).Clone();
            ICommunicationQueue result = null;
            switch (config.RoleType.ToLower())
            {
                case "consumer":
                    {
                        //result = new MqConsumerQueue(this);
                        result = new MqFastConsumerQueue(this);
                        break;
                    }
                case "producer":
                    {
                        result = new MqProductorQueue(this);
                        break;
                    }
            }
            if (result != null)
            {
                result.Config = config;
            }
            return result;

        }
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, ConnectionFactory> _mqConnectionFactoryDic =
            new System.Collections.Concurrent.ConcurrentDictionary<string, ConnectionFactory>();
        /// <summary>
        /// 创建MQ连接
        /// </summary>
        /// <param name="config">mq配置信息</param>
        /// <returns>MQ连接</returns>
        public IConnection Connection(MqConfig config)
        {
            var key = config.UserId + config.Password + config.Port + config.ServerIp;

            if (!_mqConnectionFactoryDic.ContainsKey(key))
            {
                _mqConnectionFactoryDic[key] =
                    new ConnectionFactory
                        {
                            UserName = config.UserId,
                            Password = config.Password,
                            HostName = config.ServerIp,
                            Port = config.Port,
                            VirtualHost = "/",
                            AutomaticRecoveryEnabled = true,
                            TopologyRecoveryEnabled = true,
                            RequestedChannelMax = 10,
                            UseBackgroundThreadsForIO = false
                        };
            }
            return _mqConnectionFactoryDic[key].CreateConnection();
        }

        public override string ConfigFileName
        {
            get { return "communicateQueue.config"; }
        }

        public override ICommunicationEntity GetOrCreateEntity(string findKey)
        {
            MqComunicationEntity entity;
            _entitiesPool.TryTake(out entity, 100);
            return entity ?? new MqComunicationEntity();
        }

        public ICommunicationEntity GetOrCreateEntity(byte[] caches, int length)
        {
            MqComunicationEntity buffer;
            _entitiesPool.TryTake(out buffer, 100);
            buffer = buffer ?? new MqComunicationEntity();

            if (buffer.Bytes == null)
            {
                buffer.Bytes = new byte[length];
            }
            else if (buffer.Bytes.Length < length)
            {
                Array.Resize(ref buffer.Bytes, length);
            }
            Array.Copy(caches, 0, buffer.Bytes, 0, length);

            buffer.BytesLength = length;

            return buffer;
        }

        public override void GivebackEntity(ICommunicationEntity entity)
        {
            if (_entitiesPool.Count > MaxCount) return;
            var mqEntity = entity as MqComunicationEntity;
            if (mqEntity == null) return;
            _entitiesPool.Add(mqEntity);
        }

        public override string GetRouteingString(IQueueConfig queueConfig)
        {
            var config = (MqConfig)queueConfig;
            return string.Format("{0},{1}", config.ExchangeName,
                                 string.IsNullOrEmpty(config.BindKey) ? config.RouteingKey : config.BindKey);
        }

        public override void SetRouteingString(ref IQueueConfig queueConfig, string routeingString)
        {
            var config = (MqConfig)queueConfig;
            var splits = routeingString.Split(',');
            config.RouteingKey = splits[1];
            config.BindKey = splits[1];
            config.ExchangeName = splits[0];
        }
    }
}
