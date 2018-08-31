using System.Collections.Generic;
using DES.DbCaches.Entities;

namespace DES.DbCaches.Interfaces
{
    public interface IDbProvider
    {

        /// <summary>
        /// 查找所有的转发车辆信息
        /// </summary>
        /// <returns>转发车辆信息</returns>
        List<ForwadVehicleEntity> QueryForwardVehicles();
    }
}