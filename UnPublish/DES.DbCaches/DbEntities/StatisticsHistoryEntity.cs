/* ==============================================================================
* 类型名称：StatisticsHistoryEntity  
* 类型描述：历史数据任务统计实体
* 创 建 者：linfk
* 创建日期：2017/12/22 13:43:49
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;

namespace DES.DbCaches.DbEntities
{
    /// <summary>
    /// 历史数据任务统计
    /// </summary>
    public class StatisticsHistoryEntity
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// 待统计的数量
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 任务标识
        /// </summary>
        public string TaskId { get; set; }
        /// <summary>
        /// 服务标识
        /// </summary>
        public string ServiceFlag { get; set; }
        /// <summary>
        /// 数据更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}
