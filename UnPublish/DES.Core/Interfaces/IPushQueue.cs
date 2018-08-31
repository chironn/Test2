/* ==============================================================================
 * 类型名称：IPushQueue  
 * 类型描述：数据发送队列
 * 创 建 者：linfk
 * 创建日期：2017/11/16 11:22:44
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/

namespace DES.Core.Interfaces
{
    /// <summary>
    /// 数据发送队列
    /// </summary>
    public interface IPushQueue : ICommunicationQueue
    {
        void Push(ICommunicationEntity entity);
    }
}
