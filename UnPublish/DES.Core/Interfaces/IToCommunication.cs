/* ==============================================================================
 * 类型名称：IToCommunication  
 * 类型描述：
 * 创 建 者：linfk
 * 创建日期：2017/11/29 11:36:03
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/

namespace DES.Core.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IToCommunication
    {
        string FindKey { get; }
        /// <summary>
        /// 创建该实例的服务实体
        /// </summary>
        BaseService Service { get; set; }
        void Convert(ICommunicationEntity communicationEntity, ILogicEntity entity);
    }
}
