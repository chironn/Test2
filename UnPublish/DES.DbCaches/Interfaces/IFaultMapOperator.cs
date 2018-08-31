/* ==============================================================================
* 类型名称：IFaultMapOperator  
* 类型描述：故障数据相关操作
* 创 建 者：linfk
* 创建日期：2018/1/17 13:56:56
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

namespace DES.DbCaches.Interfaces
{
    /// <summary>
    /// 故障数据相关操作
    /// </summary>
    public interface IFaultMapOperator
    {
        /// <summary>
        /// 从数据库更新缓存
        /// </summary>
        void LoadFromDb();

        /// <summary>
        /// 获取指定故障名称的故障等级
        /// </summary>
        /// <param name="protocolId">协议编号 </param>
        /// <param name="faultName">故障名称</param>
        /// <returns>查询到返回配置等级，未查到返回0</returns>
        int FaultLevel(int protocolId, string faultName);

        /// <summary>
        /// 故障列表行数
        /// </summary>
        int FaultListCount { get; }
    }
}
