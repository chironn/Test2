/* ==============================================================================
* 类型名称：Faultmap  
* 类型描述：故障码映射信息表
* 创 建 者：linfk
* 创建日期：2018/1/17 10:22:10
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

namespace DES.DbCaches.DbEntities
{
    /// <summary>
    /// 
    /// </summary>
    public class Faultmap
    {
        public int PROTOCOLID { get; set; }
        public string PROTOCOLFAULT { get; set; }
        public string FAULTCODE { get; set; }
        public int PROTOCOLFAULTLEVEL { get; set; }
    }
}
