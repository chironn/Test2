/* ==============================================================================
 * 类型名称：ILogicWork  
 * 类型描述：逻辑处理组件
 * 创 建 者：linfk
 * 创建日期：2017/11/16 16:55:51
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/

using System;
using System.Collections.Generic;

namespace DES.Core.Interfaces
{
    /// <summary>
    /// 逻辑处理组件
    /// </summary>
    public interface ILogicWork
    {
        /// <summary>
        /// 消息发布事件
        /// </summary>
        event Action<ILogicWork, ILogicEntity> PublishMessageEvent;

        /// <summary>
        /// 创建该实例的服务实体
        /// </summary>
        BaseService Service { get; set; }
        /// <summary>
        /// 逻辑处理组件
        /// </summary>
        string FindKey { get; }

        /// <summary>
        /// 订阅消息通道名称。
        /// </summary>
        List<string> SubscribeList { get; }

        /// <summary>
        /// 处理数据逻辑
        /// </summary>
        /// <param name="messge">原消息</param>
        /// <param name="entity">通信实体对象</param>
        void Dowork(ICommunicationEntity messge, ILogicEntity entity);

        /// <summary>
        /// 初始化模块
        /// </summary>
        void Initialize();
    }
}
