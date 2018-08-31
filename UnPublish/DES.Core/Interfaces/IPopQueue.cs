/* ==============================================================================
* 类型名称：IPopQueue  
* 类型描述：收到数据实体回调
* 创 建 者：linfk
* 创建日期：2017/11/16 11:22:06
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System;

namespace DES.Core.Interfaces
{
    /// <summary>
    /// 数据输出队列
    /// </summary>
    public interface IPopQueue : ICommunicationQueue
    {
        /// <summary>
        /// 收到数据实体回调
        /// <para>IMqPopQueue 队列</para>
        /// <para>CommandMsg 收到的通信实体</para>
        /// </summary>
        Action<ICommunicationEntity> ReceiveCallback { get; set; }

        /// <summary>
        /// 当前队列是否已暂停
        /// </summary>
        bool IsSuspend { get; }

        /// <summary>
        /// 暂停输出
        /// </summary>
        void Suspend();

        /// <summary>
        /// 重新开始输出
        /// </summary>
        void Restart();
    }
}
