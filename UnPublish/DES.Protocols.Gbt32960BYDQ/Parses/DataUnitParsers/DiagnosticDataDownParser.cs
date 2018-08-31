using DES.Entities.BYDQ.DataUnit;
using System.Collections.Generic;
using DES.Protocols.BYDQ.Extends;

namespace DES.Protocols.BYDQ.Parses.DataUnitParsers
{
    public class DiagnosticDataDownParser : BaseDataUnitParser
    {
        public DiagnosticDataDownParser()
        {
            ParserID = DiagnosticDataDown.ID;
        }
        public override void Decompose(byte[] message, ref int offset, Entities.BYDQ.DataBaseUnit protocol)
        {
            var entity = (DiagnosticDataDown)protocol;
            entity.SIDLength = message.ReadBigEndianUInt16(ref offset);
            int len = entity.SIDLength;//message.Length - offset - 2;
            byte[] buffer = new byte[len];
            for (int i = 0; i < len; i++)
            {
                buffer[i] = message[offset++];
            }
            entity.SIDDiagnosticData = buffer;
        }

        public override void CreateBodyBytes(Entities.BYDQ.DataBaseUnit entity, ref List<byte> buffer)
        {
            var singleEntity = (DiagnosticDataDown)entity;

            buffer.AddRange(singleEntity.SIDLength.EndianUInt16ToBytes());
            buffer.AddRange(singleEntity.SIDDiagnosticData);
        }

        public override Entities.BYDQ.DataBaseUnit CreateEntity(byte[] message, int offset)
        {
            return new DiagnosticDataDown();
        }
    }
}
