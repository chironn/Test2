using System;
using System.Collections.Generic;
using DES.Core;
using DES.Core.Interfaces;
using DES.Entities.BYDQ;
using DES.Converts.BYDQService.Entity;

namespace DES.Converts.BYDQService.Logic
{
    public class UploadLogic:ILogicWork
    {
        #region Implementation of ILogicWork

        public event Action<ILogicWork, ILogicEntity> PublishMessageEvent;

        void PubulisMesssage(BaseEntity msg)
        {
            if (PublishMessageEvent != null)
                PublishMessageEvent(null, new BaseLogicEntity<BaseEntity>
                {
                    FindKey = "BYD企标上行数据发布队列",
                    Entity = msg
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
            get { return "上行数据逻辑"; }
        }

        private readonly List<string> _subscribeList = new List<string>
            {
                "BYD企标上行数据接收队列"

            };

        public List<string> SubscribeList
        {
            get { return _subscribeList; }
        }

        /// <summary>
        /// 处理上行数据逻辑
        /// </summary>
        /// <param name="messge">原消息</param>
        /// <param name="obj">通信实体对象</param>
        public void Dowork(ICommunicationEntity messge, ILogicEntity obj)
        {
            if(obj == null) return;
            var logicEntity = (FromInComEntity)obj;
            if (logicEntity.RealEntity == null) return;
            
            PubulisMesssage(logicEntity.RealEntity);
        }

        /// <summary>
        /// 初始化模块
        /// </summary>
        public void Initialize()
        {
        }

        #endregion
    }
}
