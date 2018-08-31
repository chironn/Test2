using DES.Entities.BYDQ.DataUnit;
using System.Collections.Generic;

namespace DES.Entities.BYDQ
{
    /// <summary>
    /// 远程调试功能
    /// 云端CAN诊断-7002
    /// 云端到车机
    /// 校验码 CRC16
    /// </summary>
    [ProtoBuf.ProtoContract]
    public class RemoteDebugDown : BaseEntity
    {
        public RemoteDebugDown()
        {
            //云端到车机0x00,车机到云端0x01
            MsgType = 0x00;
            FunctionCode = 7002;
            ResponseSign = 0xFE;
            DataUnitList = new List<DiagnosticDataDown>();
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
        /// 总下发包数
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public byte CNT { get; set; }
        /// <summary>
        /// 当前下发包序号
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public byte SEQ { get; set; }
        /// <summary>
        /// 诊断数据类型
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public byte DiagnosticDataType { get; set; }
        /// <summary>
        /// 安全认证方法
        /// </summary>
        [ProtoBuf.ProtoMember(5)]
        public byte SecurityAuthentication { get; set; }
        /// <summary>
        /// 诊断模式
        /// </summary>
        [ProtoBuf.ProtoMember(6)]
        public byte DiagnosticMode { get; set; }
        /// <summary>
        /// 诊断发送CANID
        /// </summary>
        [ProtoBuf.ProtoMember(7)]
        public uint SendCANID { get; set; }
        /// <summary>
        /// 诊断接收CANID
        /// </summary>
        [ProtoBuf.ProtoMember(8)]
        public uint RecCANID { get; set; }
        /// <summary>
        /// 帧类型
        /// </summary>
        [ProtoBuf.ProtoMember(9)]
        public byte FrameType { get; set; }
        /// <summary>
        /// 被诊断模块的KeyK
        /// </summary>
        [ProtoBuf.ProtoMember(10)]
        public uint KeyK { get; set; }
        /// <summary>
        /// SID个数
        /// </summary>
        [ProtoBuf.ProtoMember(11)]
        public byte SIDCNT { get; set; }

        /// <summary>
        /// 信息体列表
        /// </summary>
        [ProtoBuf.ProtoMember(12)]
        public List<DiagnosticDataDown> DataUnitList { get; set; }

    }
}
