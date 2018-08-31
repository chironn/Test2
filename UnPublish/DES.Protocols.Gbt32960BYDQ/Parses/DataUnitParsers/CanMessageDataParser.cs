using DES.Entities.BYDQ.DataUnit;
using System;
using System.Collections.Generic;
using DES.Protocols.BYDQ.Extends;
using DES.Entities.BYDQ;

namespace DES.Protocols.BYDQ.Parses.DataUnitParsers
{
    public class CanMessageDataParser : BaseDataUnitParser
    {
        public CanMessageDataParser()
        {
            ParserID = CanMessageData.ID;
        }
        public override void Decompose(byte[] message, ref int offset, DataBaseUnit protocol)
        {
            var entity = (CanMessageData)protocol;
           //520
            entity.CANID = message.ReadBigEndianUInt32(ref offset);
            entity.FP = message[offset++];
            entity.DT = message.ReadBigEndianUInt64(ref offset); //DataTypeConversion.ConvertDateTimeToInt(message.ReadBytes6Time(ref offset));
            entity.CDL = message[offset++];
            int len = Convert.ToInt32(entity.CDL); //BitConverter.ToInt32(message, offset);
            
            byte[] buffer = new byte[8];
            try
            {
            for (int i = 0; i < 8; i++)
            {
                buffer[i] = message[offset++];
            }
            entity.CDATA = buffer;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override void CreateBodyBytes(DataBaseUnit entity, ref List<byte> buffer)
        {
            var singleEntity = (CanMessageData)entity;

            buffer.AddRange(singleEntity.CANID.EndianUInt32ToBytes());
            buffer.Add(singleEntity.FP);
            var TimeStamp = singleEntity.DT.EndianULongToBytes();
            buffer.AddRange(TimeStamp);
            buffer.Add(singleEntity.CDL);
            buffer.AddRange(singleEntity.CDATA);
        }

        public override DataBaseUnit CreateEntity(byte[] message, int offset)
        {
            return new CanMessageData();
        }
    }
}
