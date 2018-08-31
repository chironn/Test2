/* ==============================================================================
 * 类型名称：IErrorFeedBackQueue  
 * 类型描述：错误通信实体反馈队列接口，当底层通信出现问题时，将通过事件反馈给上层应用
 * 创 建 者：linfk
 * 创建日期：2018/4/5 15:55:18
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
    /// 错误通信实体反馈队列接口，当底层通信出现问题时，将通过事件反馈给上层应用
    /// </summary>
    public interface IErrorFeedbackQueue : ICommunicationQueue
    {

        /// <summary>
        /// 实体反馈事件
        /// </summary>
        event EventHandler<QueueFeedbackEventArg<IErrorFeedbackQueue, ICommunicationEntity>> FeebackEvent;

        /// <summary>
        /// 推送数据到队列
        /// </summary>
        /// <param name="entity">通信实体</param>
        void Push(ICommunicationEntity entity);

        /// <summary>
        /// 推送一组数据到通信队列
        /// </summary>
        /// <param name="entities">一组通信实体</param>
        void Push(IEnumerable<ICommunicationEntity> entities);
    }

    /// <summary>
    /// 队列实体反馈事件参数
    /// </summary>
    /// <typeparam name="TQueue">反馈数据的队列类型</typeparam>
    /// <typeparam name="ICEntity">返回的数据类型</typeparam>
    public class QueueFeedbackEventArg<TQueue, ICEntity> : EventArgs
    {
        /// <summary>
        /// 反馈数据的队列
        /// </summary>
        public TQueue FeedbackQueue { get; private set; }

        /// <summary>
        /// 反馈的数据
        /// </summary>
        public IList<ICEntity> FeedbackData { get; private set; }

        /// <summary>
        /// 构造存放一个反馈实体的实体
        /// </summary>
        /// <param name="queue">反馈数据的队列</param>
        /// <param name="entity">反馈的数据</param>
        public QueueFeedbackEventArg(TQueue queue, ICEntity entity)
        {
            FeedbackQueue = queue;
            FeedbackData = new List<ICEntity> { entity };
        }

        /// <summary>
        /// 构造存放一组反馈的实体
        /// </summary>
        /// <param name="queue">反馈数据的队列</param>
        /// <param name="entities">反馈的数据</param>
        public QueueFeedbackEventArg(TQueue queue, IEnumerable<ICEntity> entities)
        {
            FeedbackQueue = queue;
            var feedback = new List<ICEntity>();
            feedback.AddRange(entities);
            FeedbackData = feedback.AsReadOnly();
        }
    }
}
