/* ==============================================================================
 * 类型名称：IStatusQueue  
 * 类型描述：带连接状态或等待状态的队列，并会产生状态变换事件
 * 创 建 者：linfk
 * 创建日期：2018/4/8 13:36:13
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/
using System;

namespace DES.Core.Interfaces
{
    /// <summary>
    /// 带连接状态或等待状态的队列，并会产生状态变换事件
    /// </summary>
    public interface IStatusQueue : ICommunicationQueue
    {
        /// <summary>
        /// 状态变更事件
        /// </summary>
        event EventHandler<StatusChanagedArgs> StatusChangedEvent;

        /// <summary>
        /// 当前队列状态
        /// </summary>
        QueueStatus Status { get; }

        /// <summary>
        /// 设置当前队列状态
        /// </summary>
        /// <param name="status">要设置的状态</param>
        /// <exception cref="ArgumentException">当状态无法进行设置时，产生此异常</exception>
        void SetStatus(QueueStatus status);
    }


    /// <summary>
    /// 队列状态
    /// </summary>
    public enum QueueStatus
    {
        /// <summary>
        /// 未准备状态
        /// </summary>
        NotReady = 0x00,

        /// <summary>
        /// 连接中状态
        /// </summary>
        Connecting = 0x01,

        /// <summary>
        /// 连接状态
        /// </summary>
        Connected = 0x02,

        /// <summary>
        /// 准备状态。
        /// </summary>
        Ready = 0x04,

        /// <summary>
        /// 运行态
        /// </summary>
        Running = 0x08,
    }

    /// <summary>
    /// 状态变化参数
    /// </summary>
    public class StatusChanagedArgs : EventArgs
    {
        /// <summary>
        /// 状态变化队列
        /// </summary>
        public IStatusQueue Queue { get; private set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="queue">队列</param>
        public StatusChanagedArgs(IStatusQueue queue)
        {
            Queue = queue;
        }
    }

}
