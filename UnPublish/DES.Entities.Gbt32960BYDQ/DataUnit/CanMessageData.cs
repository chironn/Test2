namespace DES.Entities.BYDQ.DataUnit
{
    /// <summary>
    /// 3006数据监控CAN数据
    /// 信息类型标志为0x01
    /// </summary>
    [ProtoBuf.ProtoContract]
    public class CanMessageData : DataBaseUnit
    {
        public const byte ID = 0x01;
        public CanMessageData()
        {
            DataType = ID;
        }
        /// <summary>
        /// CAN ID
        /// </summary>
        [ProtoBuf.ProtoMember(1)]
        public uint CANID { get; set; }

        /// <summary>
        /// CANID所在CAN端口
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public byte FP { get; set; }

        /// <summary>
        /// 帧对应的时间毫秒数
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public ulong DT { get; set; }

        /// <summary>
        /// CANID报文的数据长度
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public byte CDL { get; set; }

        /// <summary>
        /// CANID报文数据
        /// </summary>
        [ProtoBuf.ProtoMember(5)]
        public byte[] CDATA { get; set; }
    }
}
