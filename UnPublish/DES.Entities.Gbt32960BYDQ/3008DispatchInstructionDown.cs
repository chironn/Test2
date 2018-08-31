namespace DES.Entities.BYDQ
{
    [ProtoBuf.ProtoContract]
    public class DispatchInstructionDown : BaseEntity
    {
        public DispatchInstructionDown()
        {
            //云端到车机0x00,车机到云端0x01
            MsgType = 0x00;
            FunctionCode = 3008;
            ResponseSign = 0xFE;
        }
        /// <summary>
        /// 消息上下行
        /// </summary>
        public byte MsgType { get; set; }

        /// <summary>
        /// 下发指令的UUID
        /// </summary>
        [ProtoBuf.ProtoMember(1)]
        public string UUID { get; set; }

        /// <summary>
        /// 时间1：年月日时分秒。与国标一致
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public byte[] Time1 { get; set; }

        /// <summary>
        /// 时间2：年月日时分秒
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public byte[] Time2 { get; set; }
    }
}
