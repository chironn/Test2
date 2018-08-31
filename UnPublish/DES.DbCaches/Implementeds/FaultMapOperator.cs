/* ==============================================================================
* 类型名称：FaultMapOperator  
* 类型描述：故障数据相关操作
* 创 建 者：linfk
* 创建日期：2018/1/17 15:46:21
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System.Collections.Generic;
using DES.DbCaches.DbEntities;
using DES.DbCaches.Interfaces;

namespace DES.DbCaches.Implementeds
{
    /// <summary>
    /// 
    /// </summary>
    public class FaultMapOperator : IFaultMapOperator
    {
        private List<Faultmap> _faultMapList;

        private System.Collections.Concurrent.ConcurrentDictionary<int, Dictionary<string, Faultmap>> _faultDic;

        public void LoadFromDb()
        {
            var dbProvider = Provider.Intance<IDbHExchangeLoad>();
            // 避免数据库异常起
            _faultMapList = dbProvider.QueryFaultMap();

            // 生成新的映射关系表
            var faultDic =
                new System.Collections.Concurrent.ConcurrentDictionary<int, Dictionary<string, Faultmap>>();
            foreach (var map in _faultMapList)
            {
                if (!faultDic.ContainsKey(map.PROTOCOLID))
                    faultDic[map.PROTOCOLID] = new Dictionary<string, Faultmap>();
                faultDic[map.PROTOCOLID][map.PROTOCOLFAULT] = map;
            }

            // 更新映射表
            _faultDic = faultDic;

        }

        public int FaultLevel(int protocolId, string faultName)
        {
            if (_faultDic == null || !_faultDic.ContainsKey(protocolId) ||
                !_faultDic[protocolId].ContainsKey(faultName))
                return 0;
            return _faultDic[protocolId][faultName].PROTOCOLFAULTLEVEL;
        }

        public int FaultListCount
        {
            get { return _faultMapList.Count; }
        }
    }
}
