/* ==============================================================================
 * 类型名称：ICommunicateQueue  
 * 类型描述：数据通信队列类型接口
 * 创 建 者：linfk
 * 创建日期：2017/11/15 16:58:30
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/

using System;

namespace DES.Core.Interfaces
{
    /// <summary>
    /// 数据通信队列类型接口
    /// </summary>
    public interface ICommunicationQueue : IDisposable
    {
        /// <summary>
        /// 当前使用的配置文件
        /// </summary>
        IQueueConfig Config { get; set; }

        /// <summary>
        /// 数据异常回调
        /// <para>IMqPopQueue 当前异常队列实体</para>
        /// <para>Exception 异常实体 </para>
        /// <para>string 扩展描述信息 </para>
        /// </summary>
        Action<ICommunicationQueue, Exception, string> ExcptionCallback { get; set; }

        /// <summary>
        /// 全局唯一查找键,建议使用配置文件中的FindKey
        /// </summary>
        string FindKey { get; }

        /// <summary>
        /// 初始化模块
        /// </summary>
        void Initialize();

    }
}
