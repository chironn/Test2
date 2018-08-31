namespace DES.Entities.BYDQ.DataUnit
{
    /// <summary>
    /// 7002远程调试上行，数据诊断
    /// 信息类型标志为0x02
    /// </summary>
    [ProtoBuf.ProtoContract]
    public class DiagnosticDataUP: DataBaseUnit
    {
        public const byte ID = 0x03;
        public DiagnosticDataUP()
        {
            DataType = ID;
        }
        /// <summary>
        /// SID数据长度
        /// </summary>
        [ProtoBuf.ProtoMember(1)]       
        public ushort SIDLength { get; set; }
        /// <summary>
        /// SID诊断结果
        /// 值           定义
        /// 0x01        诊断成功
        /// 0x02        诊断失败，超时5s
        /// 0x03        诊断失败，车辆档为0FF
        /// 0x04        诊断失败，CAN静默状态
        /// 0x05        远程CAN升级
        /// 0x06        下发命令错误
        /// 0x07        启动诊断失败
        /// 0x08        安全认证失败
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public byte[] SIDDiagnosticResult { get; set; }
    }
}
