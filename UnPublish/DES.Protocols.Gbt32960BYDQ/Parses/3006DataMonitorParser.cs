using System.Text;
using DES.DbCaches.DbEntities;
using DES.DbCaches.Entities;
using DES.Entities.BYDQ;
using System;
using System.Collections.Generic;
using System.Linq;
using DES.Protocols.BYDQ.Extends;
using DES.Entities.BYDQ.DataUnit;
using DES.Utilities.Serializies;
using Newtonsoft.Json;

namespace DES.Protocols.BYDQ.Parses
{
    public class DataMonitorParser : BaseParser
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

        public DataMonitorParser()
        {
            ParserID = 3006;
            ReflectProtocol();
        }

        /// <summary>
        /// 解析消息实体
        /// </summary>
        /// <param name="message">消息源</param>
        /// <param name="offset">偏移</param>
        /// <returns>实体</returns>
        public CanMessageData Decompose(byte[] message, ref int offset)
        {
            byte parserId = 0x01;
            
            var parser = ProtocolParseDictionay[parserId];
            // 创建消息实体
            var entity = (CanMessageData)parser.CreateEntity(message, offset);
            entity.DataType = parserId;
            parser.Decompose(message, ref offset, entity);

            return entity;
        }

        /// <summary>
        /// 向缓存数组中添加实体解析结果
        /// </summary>
        /// <param name="unit">数据单元</param>
        /// <param name="byetsBuffer">目标字节缓存</param>
        public void CreateBytes(CanMessageData unit, ref List<byte> byetsBuffer)
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
            var entity = (DataMonitor)protocol;
            if (entity.DataUnitList == null)
            {
                entity.DataUnitList = new List<CanMessageData>();
            }
            
            entity.UUID = UuidString(message, ref offset, 16);//ReadString(message, ref offset, 16);
            entity.SignType = message[offset++];
            entity.CNT = message.ReadBigEndianUInt16(ref offset);
            entity.SEQ = message.ReadBigEndianUInt16(ref offset);
            for (int i = 0; i < entity.CNT; i++)
            {
                var unit = Decompose(message, ref offset);

                if (unit == null) break;
                entity.DataUnitList.Add(unit);
            }
            //while (offset != message.Length - 2)
            //{
            //    var unit = Decompose(message, ref offset);

            //    if (unit == null) break;
            //    entity.DataUnitList.Add(unit);
            //}
            
        }

        protected override Entities.BYDQ.BaseEntity CreateEntity(byte[] message)
        {
            return new DataMonitor();
        }

        public override List<byte> CreateMessgeBody(Entities.BYDQ.BaseEntity protocol)
        {
            var buffer = new List<byte>();
            var entity = (DataMonitor)protocol;
            //WriteString(entity.UUID, 16, ref buffer);
            //buffer.AddRange(Convert.FromBase64String(entity.UUID));
            WriteUuidstring(entity.UUID, 16, ref buffer);
            buffer.Add(entity.SignType);
            buffer.AddRange(entity.CNT.EndianUInt16ToBytes());
            buffer.AddRange(entity.SEQ.EndianUInt16ToBytes());
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
        public override BYDQBaseEntity CreatEntity(BaseEntity baseEntity)
        {
            var protocol = baseEntity as DataMonitor;
            var entity = new DataMonitorEntity();
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
            entity.SignType = protocol.SignType;
            entity.DbCNT = (int)protocol.CNT;
            entity.DbSEQ = (int)protocol.SEQ;
            entity.CanMessageData = ProtoBufSerialize.Serialize(protocol.DataUnitList);//JsonConvert.SerializeObject(protocol.DataUnitList);
            return entity;
        }

        /// <summary>
        /// 数据库实体转换
        /// </summary>
        /// <param name="baseEntity"></param>
        /// <returns></returns>
        public override BaseEntity ConvertEntity(BYDQBaseEntity baseEntity)
        {
            var entity = baseEntity as DataMonitorEntity;
            var protocol = new DataMonitor();
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
            protocol.SignType = Convert.ToByte(entity.SignType);
            protocol.CNT = (ushort)entity.DbCNT;
            protocol.SEQ = (ushort)entity.DbSEQ;
            protocol.DataUnitList = ProtoBufSerialize.Deserialize<List<CanMessageData>>(entity.CanMessageData, 0, entity.CanMessageData.Length);
            return protocol;
        }
    }
}
