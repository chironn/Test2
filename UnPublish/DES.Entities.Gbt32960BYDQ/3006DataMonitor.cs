using DES.Entities.BYDQ.DataUnit;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DES.Entities.BYDQ
{
    /// <summary>
    /// 数据监控功能
    /// CAN数据多个ID混合组帧转发-3006
    /// 校验码 CRC16
    /// </summary>
    [ProtoBuf.ProtoContract]
    public class DataMonitor : BaseEntity
    {
        public DataMonitor()
        {
            FunctionCode = 3006;
            ResponseSign = 0xFF;
            DataUnitList = new List<CanMessageData>();
        }
        /// <summary>
        /// 车机将事务UUID生成后推送至云端
        /// </summary>
        [ProtoBuf.ProtoMember(1)]
        public string UUID { get; set; }

        /// <summary>
        /// 数据类型标记 ，01，02，03
        /// 0x01 :故障前后30s数据
        /// 0x02 :黑匣子数据远程配置下发调取上传
        /// 0x03 :黑匣子本地数据上传
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public byte SignType { get; set; }

        /// <summary>
        /// 总帧数
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public ushort CNT { get; set; }

        /// <summary>
        /// 包序号
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public ushort SEQ { get; set; }

        /// <summary>
        /// 信息体列表
        /// </summary>
        [ProtoBuf.ProtoMember(5)]
        public List<CanMessageData> DataUnitList { get; set; }

    }
}
