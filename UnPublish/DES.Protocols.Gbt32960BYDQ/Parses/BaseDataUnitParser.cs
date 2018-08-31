using System.Collections.Generic;
using DES.Entities.BYDQ;

namespace DES.Protocols.BYDQ.Parses
{
    /// <summary>
    /// 实时数据数据单元转换
    /// </summary>
    public abstract class BaseDataUnitParser
    {

        public byte ParserID { get; set; }
        public abstract void Decompose(byte[] message, ref int offset, DataBaseUnit entity);
        public abstract void CreateBodyBytes(DataBaseUnit entity, ref List<byte> buffer);
        public abstract DataBaseUnit CreateEntity(byte[] message, int offset);
    }
}