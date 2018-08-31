/* ==============================================================================
* 类型名称：HisotoryDbcmapEntity  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/12/29 15:04:00
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DES.DbCaches.DbEntities
{
    /// <summary>
    /// 历史数据转发实体
    /// </summary>
    public class HisotoryDbcmapEntity
    {
        /// <summary>
        /// MD5值
        /// </summary>
        public string MD5CODE { get; set; }

        /// <summary>
        /// 协议ID
        /// </summary>
        public string PROTOCOLID { get; set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        public string MAPSERIALIZE { get; set; }
    }
}
