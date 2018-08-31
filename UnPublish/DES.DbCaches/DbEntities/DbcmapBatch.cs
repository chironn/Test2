/* ==============================================================================
* 类型名称：DbcmapBatch  
* 类型描述：DBC映射关系表
* 创 建 者：linfk
* 创建日期：2017/12/5 14:32:20
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System;
namespace DES.DbCaches.DbEntities
{
    /// <summary>
    /// DBC映射关系表
    /// </summary>
    public class DbcmapBatch
    {
        /// <summary>
        /// 车辆的MD5信息
        /// </summary>
        public string MD5CODE { get; set; }

        /// <summary>
        /// DBC映射信息
        /// </summary>
        public string MAPSERIALIZE { get; set; }


        public DateTime LastLoadTime { get; set; }
    }
}
