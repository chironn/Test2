/* ==============================================================================
 * 类型名称：IParse  
 * 类型描述：
 * 创 建 者：linfk
 * 创建日期：2017/11/24 9:46:38
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
    public interface IToLogic
    {
        string FindKey { get; }
        /// <summary>
        /// 创建该实例的服务实体
        /// </summary>
        BaseService Service { get; set; }
        ILogicEntity Convert(ICommunicationEntity entity);
    }
}
