/* ==============================================================================
* 类型名称：IDbVehicleBaseInfoLoad  
* 类型描述：车辆基础信息获取相关接口
* 创 建 者：linfk
* 创建日期：2018/3/13 13:49:29
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System.Collections.Generic;
using DES.DbCaches.Entities;

namespace DES.DbCaches.Interfaces
{
    /// <summary>
    /// 车辆基础信息获取相关接口
    /// </summary>
    public interface IVehicleBaseInfoLoad
    {
        /// <summary>
        /// 加载全部车辆信息
        /// </summary>
        /// <returns>全部车辆基础信息</returns>
        List<VehicleBaseInfo> LoadAllVehicleInfo();

        /// <summary>
        /// 加载Access车辆信息
        /// </summary>
        /// <returns></returns>
        List<VehicleBaseInfo> LoadAccessVehicleInfo();
        /// <summary>
        /// 根据车架号加载指定车辆的基础信息
        /// </summary>
        /// <param name="vinno">车架号</param>
        /// <returns>指定车辆基础信息</returns>
        VehicleBaseInfo LoadAllVehicleInfo(string vinno);
    }
}
