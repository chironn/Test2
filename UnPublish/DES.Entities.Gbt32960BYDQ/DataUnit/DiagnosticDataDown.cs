namespace DES.Entities.BYDQ.DataUnit
{
    /// <summary>
    /// 7002远程调试下行，数据内容
    /// 信息类型标志为0x03
    /// </summary>
    [ProtoBuf.ProtoContract]
    public class DiagnosticDataDown : DataBaseUnit
    {
        public const byte ID = 0x02;
        public DiagnosticDataDown()
        {
            DataType = ID;
        }
        /// <summary>
        /// SID数据长度
        /// </summary>
        [ProtoBuf.ProtoMember(1)]
        public ushort SIDLength { get; set; }
        /// <summary>
        /// SID诊断数据内容
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public byte[] SIDDiagnosticData { get; set; }
    }
}
