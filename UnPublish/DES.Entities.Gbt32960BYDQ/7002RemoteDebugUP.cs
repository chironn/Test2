using DES.Entities.BYDQ.DataUnit;
using System.Collections.Generic;

namespace DES.Entities.BYDQ
{
    /// <summary>
    /// 远程调试功能
    /// 云端CAN诊断-7002
    /// 车机到云端
    /// 校验码 CRC16
    /// </summary>
    [ProtoBuf.ProtoContract]
    public class RemoteDebugUP : BaseEntity
    {
        public RemoteDebugUP()
        { 
            //云端到车机0x00,车机到云端0x01
            MsgType = 0x00;
            FunctionCode = 7002;
            //ResponseSign = 0x01; //0x01：成功；0x02：失败
            DataUnitList = new List<DiagnosticDataUP>();
        }

        /// <summary>
        /// 消息上下行
        /// </summary>
        public byte MsgType { get; set; }

        /// <summary>
        /// 诊断请求的唯一序列码
        /// </summary>
        [ProtoBuf.ProtoMember(1)]
        public string UUID { get; set; }

        /// <summary>
        /// 总上传包数
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public byte CNT { get; set; }

        /// <summary>
        /// 当前上传包序号
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public byte SEQ { get; set; }

        /// <summary>
        /// 诊断结果
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public byte DiagnosticResult { get; set; }


        /// <summary>
        /// 信息体列表
        /// </summary>
        [ProtoBuf.ProtoMember(5)]
        public List<DiagnosticDataUP> DataUnitList { get; set; }

    }
}
