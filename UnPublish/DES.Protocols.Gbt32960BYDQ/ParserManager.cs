using System;
using System.Collections.Generic;
using System.Linq;
using DES.DbCaches.Entities;
using DES.Entities.BYDQ;
using DES.Protocols.BYDQ.Extends;

namespace DES.Protocols.BYDQ
{
    /// <summary>
    /// 解析管理类型，提供基础解析方法
    /// </summary>
    public class ParserManager
    {
        /// <summary>
        /// 协议解析字典
        /// </summary>
        public Dictionary<uint, BaseParser> ProtocolParseDictionay = new Dictionary<uint, BaseParser>();

        /// <summary>
        /// 利用反射加载程序集里的协议类型和解析类型工厂
        /// </summary>
        public void ReflectProtocol(System.Reflection.Assembly assembly)
        {
            // 反射解析器
            foreach (var parser in assembly.GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface && typeof(BaseParser).IsAssignableFrom(type))
                .Select(find => (BaseParser)Activator.CreateInstance(find)))
            {
                var messageId = parser.ParserID;
                ProtocolParseDictionay[messageId] = parser;
            }


        }

        /// <summary>
        /// 遍历指定程序集合，并使用程序集已实现的解析实体，更新解析字典表
        /// </summary>
        /// <param name="assembly">需要加载的程序集</param>
        public ParserManager(System.Reflection.Assembly assembly)
            : this()
        {
            ReflectProtocol(assembly);
        }

        /// <summary>
        /// 遍历本程序集合，用“BaseParser”所在程序集已实现的解析实体，更新解析字典表
        /// </summary>
        public ParserManager()
        {
            ReflectProtocol(typeof(BaseParser).Assembly);
        }

        /// <summary>
        /// 解析协议实体
        /// </summary>
        /// <param name="message">协议消息</param>
        /// <param name="offset">消息偏移位置 </param>
        /// <returns>协议实体</returns>
        public BaseEntity Decompose(byte[] message, ref int offset)
        {

            int index = 7;
            // 读取功能号
            var msgId = message.ReadBigEndianUInt16(ref index);//Convert.ToUInt32(message[index]);// BitConverter.ToUInt32(message, index);//message[index];
            if (msgId == 7002 )
            {
                var responseSign = message.ReadByte(ref index);
                msgId = Convert.ToUInt16(responseSign == 0xFE ? 0xFE : 0xFF);
            }
            if (!ProtocolParseDictionay.ContainsKey(msgId))
                throw new InvalidOperationException(string.Format("error: msgid'{0:X2}' is not exist",
                                                                  msgId));
            // 获取解析器
            var parser = ProtocolParseDictionay[msgId];
            // 解析数据实体
            return parser.DecomposeMessage(message, ref offset);
        }

        /// <summary>
        /// 解析实体为字节数组
        /// </summary>
        /// <param name="protocol">实体对象</param>
        /// <returns>返回字节数组</returns>
        public byte[] GetBytes(BaseEntity protocol)
        {
            var functionId = protocol.FunctionCode;
            if (functionId == 7002)
            {
                var responseSign =protocol.ResponseSign;
                functionId = Convert.ToUInt16(responseSign == 0xFE ? 0xFE : 0xFF);
            }
            var parser = ProtocolParseDictionay[functionId];
            return parser.CreateMessage(protocol);
        }

        /// <summary>
        /// 解析实体为数据库实体
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public BYDQBaseEntity GetDBEntity(BaseEntity protocol)
        {
            var functionId = protocol.FunctionCode;
            if (functionId == 7002)
            {
                var responseSign = protocol.ResponseSign;
                functionId = Convert.ToUInt16(responseSign == 0xFE ? 0xFE : 0xFF);
            }
            var parser = ProtocolParseDictionay[functionId];
            return parser.CreatEntity(protocol);
        }

        /// <summary>
        /// 解析数据库实体为解析实体
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public BaseEntity GetBaseEntity(BYDQBaseEntity protocol)
        {
            var functionId = protocol.FunctionNumber;
            if (functionId == 7002)
            {
                var responseSign = protocol.ResponseSign;
                functionId = Convert.ToUInt16(responseSign == 0xFE ? 0xFE : 0xFF);
            }
            var parser = ProtocolParseDictionay[Convert.ToUInt32(functionId)];
            return parser.ConvertEntity(protocol);
        }
    }
}
