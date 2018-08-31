/* ==============================================================================
* 类型名称：RealTimeDataMsg  
* 类型描述：实时数据通信实体
* 创 建 者：linfk
* 创建日期：2017/12/14 15:59:35
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System.Collections.Generic;
using ProtoBuf;

namespace DES.Utilities
{
    [ProtoContract]
    public class RealTimeDataMsg
    {
        [ProtoMember(5)]
        public double[] CD_Arr { get; set; }

        [ProtoMember(7)]
        public double[] DD_Arr { get; set; }

        [ProtoMember(10)]
        public string DK { get; set; }

        [ProtoMember(9990)]
        public Dictionary<string, string> EXT { get; set; }

        /// <summary>
        /// 字节数据扩展项
        /// </summary>
        [ProtoMember(9991)]
        public Dictionary<string, byte[]> EXTA { get; set; }

        [ProtoMember(9992)]
        public Dictionary<string, double> EXTB { get; set; }

        [ProtoMember(12)]
        public string FLAG { get; set; }

        [ProtoMember(6)]
        public string MD { get; set; }

        [ProtoMember(8)]
        public double[] OT_Arr { get; set; }

        [ProtoMember(2)]
        public string RE { get; set; }

        [ProtoMember(9)]
        public uint[] ST_Arr { get; set; }

        [ProtoMember(3)]
        public uint TC { get; set; }

        [ProtoMember(11)]
        public string TE { get; set; }

        /// <summary>
        /// 车架号
        /// </summary>
        [ProtoMember(4)]
        public string VIN { get; set; }

        [ProtoMember(1)]
        public uint VS { get; set; }
    }
}
