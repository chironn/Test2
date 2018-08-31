/* ==============================================================================
* 类型名称：ForwadVehicleEntity  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/12/18 13:17:58
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

namespace DES.DbCaches.Entities
{
    /// <summary>
    /// 转发车辆信息
    /// </summary>
    public class ForwadVehicleEntity
    {
        /// <summary>
        /// 车架号     
        /// </summary>
        public string VehicleId { get; set; }

        /// <summary>
        /// 所属于的转发SEQ   
        /// </summary>
        public string Seq { get; set; }

        /// <summary>
        /// 转发的URL信息
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 转发端口
        /// </summary>
        public int Port { get; set; }
    }
}
