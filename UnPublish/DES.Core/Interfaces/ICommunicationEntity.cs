/* ==============================================================================
 * 类型名称：ICommunicationEntity  
 * 类型描述：通信实体接口
 * 创 建 者：linfk
 * 创建日期：2017/11/17 9:01:17
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/

namespace DES.Core.Interfaces
{
    /// <summary>
    /// 通信实体接口
    /// </summary>
    public interface ICommunicationEntity
    {
        /// <summary>
        /// 实体标识查找键
        /// </summary>
        string FindKey { get; set; }

        /// <summary>
        /// 为当前实体设置路由信息
        /// </summary>
        /// <param name="routeing"></param>
        void SetRouteing(string routeing);

        /// <summary>
        /// 读取当前实体的路由信息
        /// </summary>
        /// <returns></returns>
        string GetRoteing();
    }
}
