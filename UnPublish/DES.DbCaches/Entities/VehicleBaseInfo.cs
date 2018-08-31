/* ==============================================================================
* 类型名称：VehicleBaseInfo  
* 类型描述：车辆基础信息
* 创 建 者：linfk
* 创建日期：2018/3/14 10:56:01
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System;
namespace DES.DbCaches.Entities
{
    /// <summary>
    /// 记录车辆基础信息的接口实体
    /// </summary>
    public class VehicleBaseInfo
    {
        /// <summary>
        /// 车架号
        /// </summary>
        public string Vinno { get; set; }

        /// <summary>
        /// 车牌号信息
        /// </summary>
        public string LicensePlate { get; set; }

        /// <summary>
        /// 车辆绑定终端号
        /// </summary>
        public uint Terminalcode { get; set; }

        /// <summary>
        /// 转发接入用户名
        /// </summary>
        public string Accessinfoid { get; set; }

    }
}
