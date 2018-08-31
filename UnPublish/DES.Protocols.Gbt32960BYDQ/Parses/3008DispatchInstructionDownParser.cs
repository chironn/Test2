using System;
using DES.DbCaches.DbEntities;
using DES.DbCaches.Entities;
using DES.Entities.BYDQ;
using System.Collections.Generic;
using DES.Protocols.BYDQ.Extends;
namespace DES.Protocols.BYDQ.Parses
{
    public class DispatchInstructionDownParser : BaseParser
    {
        public DispatchInstructionDownParser()
        {
            ParserID = 3008;
        }
        protected override void OnDecomposeBody(byte[] message, ref int offset, BaseEntity protocol)
        {
            var entity = (DispatchInstructionDown)protocol;
            entity.UUID = UuidString(message, ref offset, 16);
            entity.Type = message.ReadByte(ref offset);
            byte[] buffer1 = new byte[6];
            byte[] buffer2 = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                buffer1[i] = message[offset++];
            }
            entity.Time1 = buffer1;
            for (int j = 0; j < 6; j++)
            {
                buffer2[j] = message[offset++];
            }
            entity.Time2 = buffer2;
        }

        protected override BaseEntity CreateEntity(byte[] message)
        {
            return new DispatchInstructionDown();
        }

        public override List<byte> CreateMessgeBody(Entities.BYDQ.BaseEntity protocol)
        {
            var sendBuffer = new List<byte>(50);
            var singleEntity = (DispatchInstructionDown)protocol;
            //WriteBase64String(singleEntity.UUID, 16, ref sendBuffer);
            WriteUuidstring(singleEntity.UUID, 16, ref sendBuffer);
            //sendBuffer.Add(singleEntity.Type);
            sendBuffer.AddRange(singleEntity.Time1);
            sendBuffer.AddRange(singleEntity.Time2);
            return sendBuffer;
        }

        public override DbCaches.Entities.BYDQBaseEntity CreatEntity(BaseEntity baseEntity)
        {
            int offset = 0;
            var protocol = baseEntity as DispatchInstructionDown;
            var entity = new DispatchInstructionDownEntity();
            entity.Encryption = protocol.Encryption;
            entity.Type = protocol.Type;
            entity.CMD = protocol.CMD;
            entity.FunctionNumber = (int)protocol.FunctionCode;
            entity.ResponseSign = protocol.ResponseSign;
            entity.FunctionStatus = protocol.FunctionStatus;
            entity.UniqueIdentity = protocol.UniqueIdentity;
            entity.ProductType = protocol.ProductType;
            entity.CloudProductCode = protocol.CloudProductCode;
            entity.TimeStamp = protocol.TimeStamp.ToString();
            entity.Time = BytesExtend.ConvertLongToDateTime(long.Parse(protocol.TimeStamp.ToString()));
            entity.UUID = protocol.UUID;
            entity.Time1 = protocol.Time1.ReadBytes6Time(ref offset);
            offset = 0;
            entity.Time2 = protocol.Time2.ReadBytes6Time(ref offset);
            return entity;
        }

        public override BaseEntity ConvertEntity(BYDQBaseEntity baseEntity)
        {
            var entity = baseEntity as DispatchInstructionDownEntity;
            var protocol = new DispatchInstructionDown();
            protocol.Encryption = Convert.ToByte(entity.Encryption);
            protocol.Type = Convert.ToByte(entity.Type);
            protocol.CMD = Convert.ToByte(entity.CMD);
            protocol.FunctionCode = (ushort)entity.FunctionNumber;
            protocol.ResponseSign = Convert.ToByte(entity.ResponseSign);
            protocol.FunctionStatus = Convert.ToByte(entity.FunctionStatus);
            protocol.UniqueIdentity = entity.UniqueIdentity;
            protocol.ProductType = Convert.ToByte(entity.ProductType);
            protocol.CloudProductCode = Convert.ToByte(entity.CloudProductCode);
            protocol.TimeStamp = ulong.Parse(entity.TimeStamp);//(ulong)BytesExtend.ConvertDataTimeToLong(entity.Time);
            protocol.UUID = entity.UUID;
            protocol.Time1 = entity.Time1.ToBytes6();
            protocol.Time2 = entity.Time2.ToBytes6();
            return protocol;
        }
    }
}
