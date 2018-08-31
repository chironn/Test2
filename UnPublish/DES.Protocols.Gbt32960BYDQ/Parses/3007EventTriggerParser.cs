using System;
using System.Diagnostics;
using DES.DbCaches.DbEntities;
using DES.Entities.BYDQ;
using System.Collections.Generic;
using DES.Protocols.BYDQ.Extends;
using DES.DbCaches.Entities;
using Newtonsoft.Json;

namespace DES.Protocols.BYDQ.Parses
{
    public class EventTriggerParser : BaseParser
    {
        public EventTriggerParser()
        {
            ParserID = 3007;
        }

        protected override void OnDecomposeBody(byte[] message, ref int offset, Entities.BYDQ.BaseEntity protocol)
        {
            var entity = (EventTrigger)protocol;
            entity.UUID = UuidString(message, ref offset, 16);//ReadString(message, ref offset, 16);
            entity.DT = message.ReadBigEndianUInt64(ref offset);
            entity.EventCode = message.ReadByte(ref offset);
        }

        protected override Entities.BYDQ.BaseEntity CreateEntity(byte[] message)
        {
            return new EventTrigger();
        }

        public override List<byte> CreateMessgeBody(Entities.BYDQ.BaseEntity protocol)
        {
            var sendBuffer = new List<byte>(27);
            var singleEntity = (EventTrigger)protocol;
            WriteUuidstring(singleEntity.UUID, 16, ref sendBuffer);
            //sendBuffer.AddRange(Convert.FromBase64String(singleEntity.UUID));
            //WriteString(singleEntity.UUID, 16, ref sendBuffer);
            sendBuffer.AddRange(singleEntity.DT.EndianULongToBytes());
            sendBuffer.Add(singleEntity.EventCode);
            return sendBuffer;
        }

        /// <summary>
        /// 生成数据库实体
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <returns></returns>
        public override BYDQBaseEntity CreatEntity(BaseEntity baseEntity)
        {
            var protocol = baseEntity as EventTrigger;
            var entity = new EventTriggerEntity();
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
            entity.DbDT = protocol.DT.ToString();
            entity.EventCode = protocol.EventCode;
            return entity;
        }

        /// <summary>
        /// 数据库实体转换
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <returns></returns>
        public override BaseEntity ConvertEntity(BYDQBaseEntity baseEntity)
        {
            var entity = baseEntity as EventTriggerEntity;
            var protocol = new EventTrigger();
            protocol.Encryption = Convert.ToByte(entity.Encryption);
            protocol.Type = Convert.ToByte(entity.Type);
            protocol.CMD = Convert.ToByte(entity.CMD);
            protocol.FunctionCode = (ushort)entity.FunctionNumber;
            protocol.ResponseSign =  Convert.ToByte(entity.ResponseSign);
            protocol.FunctionStatus = Convert.ToByte(entity.FunctionStatus);
            protocol.UniqueIdentity = entity.UniqueIdentity;
            protocol.ProductType =  Convert.ToByte(entity.ProductType);
            protocol.CloudProductCode = Convert.ToByte(entity.CloudProductCode);
            protocol.TimeStamp = (ulong)BytesExtend.ConvertDataTimeToLong(entity.Time);
            protocol.UUID = entity.UUID;
            protocol.DT = Convert.ToUInt64(entity.DbDT);
            protocol.EventCode = Convert.ToByte(entity.EventCode);
            return protocol;
        }
    }
}
