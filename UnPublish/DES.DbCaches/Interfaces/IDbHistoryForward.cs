/* ==============================================================================
* 类型名称：IDbHistoryForward  
* 类型描述：历史数据转发相关操作接口
* 创 建 者：linfk
* 创建日期：2017/12/22 13:36:00
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System.Collections.Generic;
using DES.DbCaches.DbEntities;

namespace DES.DbCaches.Interfaces
{
    /// <summary>
    /// 历史数据转发相关操作接口
    /// </summary>
    public interface IDbHistoryForward
    {
        /// <summary>
        /// 当前统计信息字典，键为任务ID
        /// </summary>
        IDictionary<string, StatisticsHistoryEntity> StatisticsDic { get; }

        /// <summary>
        /// 记录统计数据
        /// </summary>
        /// <param name="entity">统计数据实体</param>
        void StatisticsTask(StatisticsHistoryEntity entity);

        /// <summary>
        /// 初始化实体
        /// </summary>
        void Initialize();
    }
}
