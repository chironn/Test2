using System.Collections.Generic;
using DES.Protocols.BYDQ.Extends;
using DES.Entities.BYDQ.DataUnit;

namespace DES.Protocols.BYDQ.Parses.DataUnitParsers
{
    public class DiagnosticDataUPParser : BaseDataUnitParser
    {
        public DiagnosticDataUPParser()
        {
            ParserID = DiagnosticDataUP.ID;
        }
        public override void Decompose(byte[] message, ref int offset, Entities.BYDQ.DataBaseUnit protocol)
        {
            var entity = (DiagnosticDataUP)protocol;
            entity.SIDLength = message.ReadBigEndianUInt16(ref offset);
            int len = entity.SIDLength;//BitConverter.ToInt32(message, offset);
            byte[] buffer = new byte[len];
            for (int i = 0; i < len; i++)
            {
                buffer[i] = message[offset++];
            }
            entity.SIDDiagnosticResult = buffer;
        }

        public override void CreateBodyBytes(Entities.BYDQ.DataBaseUnit entity, ref List<byte> buffer)
        {
            var singleEntity = (DiagnosticDataUP)entity;

            buffer.AddRange(singleEntity.SIDLength.EndianUInt16ToBytes());
            buffer.AddRange(singleEntity.SIDDiagnosticResult);
        }

        public override Entities.BYDQ.DataBaseUnit CreateEntity(byte[] message, int offset)
        {
            return new DiagnosticDataUP();
        }
    }
}
