/* ==============================================================================
 * 类型名称：IQueueConfig  
 * 类型描述：
 * 创 建 者：linfk
 * 创建日期：2017/11/15 17:02:29
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/

namespace DES.Core.Interfaces
{
    /// <summary>
    /// 队列配置信息
    /// </summary>
    public interface IQueueConfig
    {
        /// <summary>
        /// 全局唯一查找键
        /// </summary>
        string FindKey { get; set; }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        string ToString();

        /// <summary>
        /// 读取路由信息
        /// </summary>
        /// <returns>路由信息</returns>
        string GetRouteing();
    }
}
