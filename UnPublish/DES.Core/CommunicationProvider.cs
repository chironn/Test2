/* ==============================================================================
 * 类型名称：CommunicateProvider  
 * 类型描述：数据通信队工厂类
 * 创 建 者：linfk
 * 创建日期：2017/11/15 16:58:30
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/

using System.Collections.Generic;
using System.Linq;
using DES.Core.Interfaces;

namespace DES.Core
{
    /// <summary>
    /// 数据通信队工厂类
    /// </summary>
    public abstract class CommunicationProvider
    {

        /// <summary>
        /// 日志记录对象
        /// </summary>
        public ILogWrite LogWrite { get; set; }

        /// <summary>
        /// 当前Provider启动过的通信队列字典
        /// </summary>
        public System.Collections.Concurrent.ConcurrentDictionary<string, ICommunicationQueue> RunQueueDic { get; protected set; }

        /// <summary>
        /// 启动队列
        /// </summary>
        /// <param name="queue">队列</param>
        protected virtual void OnRunQueue(ICommunicationQueue queue)
        {
            queue.Initialize();
            if (RunQueueDic == null)
                RunQueueDic = new System.Collections.Concurrent.ConcurrentDictionary<string, ICommunicationQueue>();
            RunQueueDic[queue.FindKey] = queue;
        }

        /// <summary>
        ///  所使用的配置文件名称，主框架会用该名称加载配置文件
        /// </summary>
        public abstract string ConfigFileName { get; }

        /// <summary>
        /// 获取一个通信实体
        /// </summary>
        /// <param name="findKey">查找键，用于判断获取哪个类型队列的通信实体</param>
        /// <returns>通信实体</returns>
        public abstract ICommunicationEntity GetOrCreateEntity(string findKey);

        /// <summary>
        /// 归还一个通信实体
        /// </summary>
        /// <param name="entity"></param>
        public abstract void GivebackEntity(ICommunicationEntity entity);

        /// <summary>
        /// 从队列配置中读取路由字符串信息
        /// </summary>
        /// <param name="queueConfig">队列配置文件</param>
        /// <returns>路由字符串信息</returns>
        public abstract string GetRouteingString(IQueueConfig queueConfig);
        /// <summary>
        /// 设置配置信息的路由信息
        /// </summary>
        /// <param name="queueConfig">队列配置信息</param>
        /// <param name="routeingString">路由字符串</param>
        public abstract void SetRouteingString(ref IQueueConfig queueConfig, string routeingString);


        /// <summary>
        /// 读取连接字符串，创建队列配置文件
        /// </summary>
        /// <param name="context">配置文件内容</param>
        /// <returns>返回加载到的配置文件</returns>
        public abstract List<IQueueConfig> CreateConfig(string context);

        /// <summary>
        /// 根据配置文件创佳妮队列
        /// </summary>
        /// <param name="config">配置文件</param>
        /// <returns>通信队列</returns>
        public abstract ICommunicationQueue CreateQueue(IQueueConfig config);
        /// <summary>
        /// 启动队列
        /// </summary>
        /// <param name="queue">队列</param>
        public void RunQueue(ICommunicationQueue queue)
        {
            OnRunQueue(queue);
        }

        public void DisposeQueue()
        {
            var keys = RunQueueDic.Keys.ToArray();
            foreach (var key in keys)
            {
                ICommunicationQueue queue;
                RunQueueDic.TryRemove(key, out queue);
                if (queue != null)
                    queue.Dispose();
            }
        }
    }
}
