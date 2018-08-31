using DES.DbCaches.DbEntities;
/* ==============================================================================
* 类型名称：IDbHExchangeLoad  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/12/29 14:59:17
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System.Collections.Generic;

namespace DES.DbCaches.Interfaces
{
    /// <summary>
    /// 历史数据转发加载操作接口
    /// </summary>
    public interface IDbHExchangeLoad
    {
        /// <summary>
        /// 查询历史数不
        /// </summary>
        /// <param name="md5codes">MD5值信息</param>
        /// <returns></returns>
        List<HisotoryDbcmapEntity> QueryHistoryDbcmap(IEnumerable<string> md5codes);

        /// <summary>
        /// 获取全局故障信息列表
        /// </summary>
        /// <returns>全局故障信息列表</returns>
        List<Faultmap> QueryFaultMap();
    }
}
