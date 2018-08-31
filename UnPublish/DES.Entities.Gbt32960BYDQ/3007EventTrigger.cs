namespace DES.Entities.BYDQ
{
    /// <summary>
    /// 事件触发描述信息
    /// 事件触发描述信息上传-3007
    /// 校验码 CRC16
    /// </summary>
    [ProtoBuf.ProtoContract]
    public class EventTrigger : BaseEntity
    {
        public EventTrigger()
        {
            FunctionCode = 3007;
            ResponseSign = 0xFF;
        }

        /// <summary>
        /// 车机将事务UUID生成后推送至云端
        /// </summary>
        [ProtoBuf.ProtoMember(1)]
        public string UUID { get; set; }

        /// <summary>
        /// 事件触发对应时间毫秒数
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public ulong DT { get; set; }

        /// <summary>
        /// 故障码
        /// EventCode             描述
        /// 0x01                SRS故障-0x08c
        /// 0x02                BS故障-0x122
        /// 0x03                驻车故障-0x30d
        /// 0x04                驻车故障-0x122
        /// 0x05                驻车故障-0x218
        /// 0x06                转向故障-0x24c
        /// 0x07                冷区液温度过高-0x30d
        /// 0x08                动力系统故障-0x240
        /// 0x09                充电系统故障-0x26c
        /// 0x0A                充电系统故障-0x30d
        /// 0x0B                充电系统故障-0x449
        /// 0x0C                充电系统故障-0x449
        /// 0x0D                充电系统故障-0x449
        /// 0x0E                动力电子故障-0x224
        /// 0x0F                动力电池过热-0x244
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public byte EventCode { get; set; }

    }
}
