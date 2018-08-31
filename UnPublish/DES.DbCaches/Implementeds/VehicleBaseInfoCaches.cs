/* ==============================================================================
* 类型名称：VehicleBaseInfoCaches  
* 类型描述：车辆基础信息加载缓存，第一次查询会调用“DapperDbProvider”从数据库读取信息，加载完成后直接从缓存中读取数据，目前该功能呢为历史数据转发，所以缓存不进行刷新。后期可考虑定时刷新。
* 创 建 者：linfk
* 创建日期：2018/3/14 13:35:52
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System.Collections.Generic;
using DES.DbCaches.Interfaces;

namespace DES.DbCaches.Implementeds
{
    /// <summary>
    /// 车辆基础信息加载缓存，第一次查询会调用“DapperDbProvider”从数据库读取信息，加载完成后直接从缓存中读取数据，目前该功能呢为历史数据转发，所以缓存不进行刷新。后期可考虑定时刷新。
    /// </summary>
    public class VehicleBaseInfoCaches : IVehicleBaseInfoLoad
    {

        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, Entities.VehicleBaseInfo> _vehicleBaseInfoDic =
            new System.Collections.Concurrent.ConcurrentDictionary<string, Entities.VehicleBaseInfo>();


        private readonly DapperDbProvider _dbProvider = new DapperDbProvider();


        public List<Entities.VehicleBaseInfo> LoadAllVehicleInfo()
        {
            // 直接返回数据库查询结果
            return _dbProvider.LoadAllVehicleInfo();
        }
        public List<Entities.VehicleBaseInfo> LoadAccessVehicleInfo()
        {
            //直接返回查询结果
            return _dbProvider.LoadAccessVehicleInfo();
        }
        public Entities.VehicleBaseInfo LoadAllVehicleInfo(string vinno)
        {
            Entities.VehicleBaseInfo vehicleBaseInfo;
            // 先从缓存中读取
            if (!_vehicleBaseInfoDic.TryGetValue(vinno, out vehicleBaseInfo))
            {
                // 读取不到从数据库里读取
                vehicleBaseInfo = _dbProvider.LoadAllVehicleInfo(vinno);

                if (vehicleBaseInfo != null)
                {
                    // 读取到数据则更新缓存
                    _vehicleBaseInfoDic.AddOrUpdate(vinno, vehicleBaseInfo, (i, v) => vehicleBaseInfo);
                }
            }

            return vehicleBaseInfo;
        }

    }
}
