using System;
using System.Collections.Generic;
using DES.Core;
using DES.Core.Interfaces;
using DES.Converts.BYDQService.Entity;

namespace DES.Converts.BYDQService.Logic
{
    public class DownloadLogic : ILogicWork
    {
        #region Implementation of ILogicWork

        public event Action<ILogicWork, ILogicEntity> PublishMessageEvent;

        void PubulisMesssage(byte[] msg)
        {
            if (PublishMessageEvent != null)
                PublishMessageEvent(null, new BaseLogicEntity<FromIncomBufferEntity>
                {
                    FindKey = "BYD企标下行数据发布队列",
                    Entity = new FromIncomBufferEntity {
                        FindKey = "BYD企标下行数据发布队列",
                        Buffer = msg
                    }
                });
        }
        /// <summary>
        /// 创建该实例的服务实体
        /// </summary>
        public BaseService Service { get; set; }

        /// <summary>
        /// 逻辑处理组件
        /// </summary>
        public string FindKey
        {
            get { return "下行数据逻辑"; }
        }

        private readonly List<string> _subscribeList = new List<string>
            {
                "BYD企标下行数据接收队列"

            };

        public List<string> SubscribeList
        {
            get { return _subscribeList; }
        }

        
        /// <summary>
        /// 初始化模块
        /// </summary>
        public void Initialize()
        {
        }

        #endregion

        /// <summary>
        /// 处理下行数据逻辑
        /// </summary>
        public void Dowork(ICommunicationEntity messge, ILogicEntity entity)
        {
            if(entity == null) return;
            var logicEntity = (FromIncomBufferEntity)entity;
            if (logicEntity.Buffer == null) return;
            
            PubulisMesssage(logicEntity.Buffer);
        }
    }
}
