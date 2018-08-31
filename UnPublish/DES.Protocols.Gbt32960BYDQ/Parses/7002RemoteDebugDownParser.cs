using DES.DbCaches.DbEntities;
using DES.DbCaches.Entities;
using DES.Entities.BYDQ;
using System;
using System.Collections.Generic;
using System.Linq;
using DES.Protocols.BYDQ.Extends;
using DES.Entities.BYDQ.DataUnit;
using DES.Utilities.Serializies;

namespace DES.Protocols.BYDQ.Parses
{
    public class RemoteDebugDownParser:BaseParser
    {
        /// <summary>
        /// 协议解析字典
        /// </summary>
        public Dictionary<byte, BaseDataUnitParser> ProtocolParseDictionay = new Dictionary<byte, BaseDataUnitParser>();

        /// <summary>
        /// 利用反射加载程序集里的协议类型和解析类型工厂
        /// </summary>
        public void ReflectProtocol()
        {
            ReflectProtocol(typeof(BaseDataUnitParser).Assembly);
        }

        public void ReflectProtocol(System.Reflection.Assembly assembly)
        {
            // 反射解析器
            foreach (var parser in assembly.GetTypes()
                  .Where(type => type.BaseType == typeof(BaseDataUnitParser))
                  .Select(find => (BaseDataUnitParser)Activator.CreateInstance(find)))
            {
                var messageId = parser.ParserID;
                ProtocolParseDictionay[messageId] = parser;
            }
        }

        public RemoteDebugDownParser()
        {
            ParserID = Convert.ToUInt32(0xFE);//7002;
            ReflectProtocol();
        }

        /// <summary>
        /// 解析消息实体
        /// </summary>
        /// <param name="message">消息源</param>
        /// <param name="offset">偏移</param>
        /// <returns>实体</returns>
        public DiagnosticDataDown Decompose(byte[] message, ref int offset)
        {
            byte parserId = 0x02;
            var parser = ProtocolParseDictionay[parserId];
            // 创建消息实体
            var entity = (DiagnosticDataDown)parser.CreateEntity(message, offset);
            entity.DataType = parserId;
            parser.Decompose(message, ref offset, entity);

            return entity;
        }

        /// <summary>
        /// 向缓存数组中添加实体解析结果
        /// </summary>
        /// <param name="unit">数据单元</param>
        /// <param name="byetsBuffer">目标字节缓存</param>
        public void CreateBytes(DataBaseUnit unit, ref List<byte> byetsBuffer)
        {
            var parserId = unit.DataType;
            if (!ProtocolParseDictionay.ContainsKey(parserId))
                throw new InvalidOperationException(string.Format("error: msgid'{0:X2}' is not exist",
                                                                  parserId));
            var parser = ProtocolParseDictionay[parserId];
            //byetsBuffer.Add(unit.DataType);
            parser.CreateBodyBytes(unit, ref byetsBuffer);
        }

        protected override void OnDecomposeBody(byte[] message, ref int offset, Entities.BYDQ.BaseEntity protocol)
        {
            var entity = (RemoteDebugDown)protocol;
            if (entity.DataUnitList == null)
                entity.DataUnitList = new List<DiagnosticDataDown>();
            entity.UUID = UuidString(message, ref offset, 16);
            entity.CNT = message.ReadByte(ref offset);
            entity.SEQ = message.ReadByte(ref offset);
            entity.DiagnosticDataType = message.ReadByte(ref offset);
            entity.SecurityAuthentication = message.ReadByte(ref offset); ;
            entity.DiagnosticMode = message.ReadByte(ref offset);
            entity.SendCANID = message.ReadBigEndianUInt32(ref offset);
            entity.RecCANID = message.ReadBigEndianUInt32(ref offset);
            entity.FrameType = message.ReadByte(ref offset);
            entity.KeyK = message.ReadBigEndianUInt32(ref offset);
            entity.SIDCNT = message.ReadByte(ref offset);
            int length = Convert.ToInt32(entity.SIDCNT);
            for (int i = 0; i < length; i++)
            {
                var unit = Decompose(message, ref offset);
                if (unit == null) break;
                entity.DataUnitList.Add(unit);
            }
            //while (offset != message.Length - 1)
            //{
            //    var unit = Decompose(message, ref offset);
            //    if (unit == null) break;
            //    entity.DataUnitList.Add(unit);
            //}
        }

        protected override Entities.BYDQ.BaseEntity CreateEntity(byte[] message)
        {
            return new RemoteDebugDown();
        }

        public override List<byte> CreateMessgeBody(Entities.BYDQ.BaseEntity protocol)
        {
            var buffer = new List<byte>();
            var entity = (RemoteDebugDown)protocol;
            //buffer.Add(entity.MsgType);
            WriteUuidstring(entity.UUID, 16, ref buffer);
            buffer.Add(entity.CNT);
            buffer.Add(entity.SEQ);
            buffer.Add(entity.DiagnosticDataType);
            buffer.Add(entity.SecurityAuthentication);
            buffer.Add(entity.DiagnosticMode);
            buffer.AddRange(entity.SendCANID.EndianUInt32ToBytes());
            buffer.AddRange(entity.RecCANID.EndianUInt32ToBytes());
            buffer.Add(entity.FrameType);
            buffer.AddRange(entity.KeyK.EndianUInt32ToBytes());
            buffer.Add(entity.SIDCNT);
            if (entity.DataUnitList != null)
            {
                foreach (var unit in entity.DataUnitList)
                {
                    CreateBytes(unit, ref buffer);
                }
            }
            return buffer;
        }
        /// <summary>
        /// 生成数据库实体
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <returns></returns>
        public override DbCaches.Entities.BYDQBaseEntity CreatEntity(BaseEntity baseEntity)
        {
            var protocol = baseEntity as RemoteDebugDown;
            var entity = new RemoteDebugDownEntity();
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
            entity.CNT = (int)protocol.CNT;
            entity.SEQ = (int)protocol.SEQ;
            entity.DiagnosticDataType = (int)protocol.DiagnosticDataType;
            entity.SecurityAuthentication = (int)protocol.SecurityAuthentication;
            entity.DiagnosticMode = (int)protocol.DiagnosticMode;
            entity.SendCANID = (int)protocol.SendCANID;
            entity.RecCANID = (int)protocol.RecCANID;
            entity.FrameType = (int)protocol.FrameType;
            entity.KeyK = (int)protocol.KeyK;
            entity.SIDCNT = (int)protocol.SIDCNT;
            entity.DiagnosticDataList = ProtoBufSerialize.Serialize(protocol.DataUnitList);//JsonConvert.SerializeObject(protocol.DataUnitList);
            return entity;
        }
        /// <summary>
        /// 数据库实体转换
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <returns></returns>
        public override BaseEntity ConvertEntity(BYDQBaseEntity baseEntity)
        {
            var entity = baseEntity as RemoteDebugDownEntity;
            var protocol = new RemoteDebugDown();
            protocol.Encryption = Convert.ToByte(entity.Encryption);
            protocol.Type = Convert.ToByte(entity.Type);
            protocol.CMD = Convert.ToByte(entity.CMD);
            protocol.FunctionCode = (ushort)entity.FunctionNumber;
            protocol.ResponseSign = Convert.ToByte(entity.ResponseSign);
            protocol.FunctionStatus = Convert.ToByte(entity.FunctionStatus);
            protocol.UniqueIdentity = entity.UniqueIdentity;
            protocol.ProductType = Convert.ToByte(entity.ProductType);
            protocol.CloudProductCode = Convert.ToByte(entity.CloudProductCode);
            protocol.TimeStamp = (ulong)BytesExtend.ConvertDataTimeToLong(entity.Time);
            protocol.UUID = entity.UUID;
            protocol.CNT = Convert.ToByte(entity.CNT);
            protocol.SEQ = Convert.ToByte(entity.SEQ);
            protocol.CNT = Convert.ToByte(entity.CNT);
            protocol.SEQ = Convert.ToByte(entity.SEQ);
            protocol.DiagnosticDataType = Convert.ToByte(entity.DiagnosticDataType);
            protocol.SecurityAuthentication = Convert.ToByte(entity.SecurityAuthentication);
            protocol.DiagnosticMode = Convert.ToByte(entity.DiagnosticMode);
            protocol.SendCANID = (uint)entity.SendCANID;
            protocol.RecCANID = (uint)entity.RecCANID;
            protocol.FrameType = Convert.ToByte(entity.FrameType);
            protocol.KeyK = (uint)entity.KeyK;
            protocol.SIDCNT = Convert.ToByte(entity.SIDCNT);
            protocol.DataUnitList = ProtoBufSerialize.Deserialize<List<DiagnosticDataDown>>(entity.DiagnosticDataList, 0, entity.DiagnosticDataList.Length);
            return protocol;
        }
    }
}
