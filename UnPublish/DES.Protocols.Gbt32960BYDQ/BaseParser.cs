using System.ComponentModel;
using DES.DbCaches.Entities;
using DES.Entities.BYDQ;
using System;
using System.Collections.Generic;
using System.Text;
using DES.Protocols.BYDQ.Extends;

namespace DES.Protocols.BYDQ
{
    /// <summary>
    /// 协议转换基础抽象类
    /// </summary>
    public abstract class BaseParser
    {
        #region

        /// <summary>
        /// 生成校验码
        /// </summary>
        /// <param name="result"></param>
        /// <param name="bt"></param>
        public static void Xor(ref byte result, byte bt)
        {
            result = BitConverter.GetBytes(result ^ bt)[0];
        }

        /// <summary>
        /// 帧起始标记
        /// </summary>
        public static byte[] FrameStartSign = new byte[] { 0xFE, 0xFE };

        /// <summary>
        /// 协议默认编码方式,UTF8
        /// </summary>
        public static readonly Encoding DefaultEcoding = Encoding.GetEncoding("utf-8");

        /// <summary>
        /// byte 转换为十六进制字符串
        /// </summary>
        /// <param name="inBytes"></param>
        /// <returns></returns>
        public static string ToHex(byte[] inBytes)
        {
            var stringOut = BitConverter.ToString(inBytes).Replace("-", "");
            return stringOut.Trim();
        }

        /// <summary>
        /// 使用协议编码方式读取字符串
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="offset">偏移位置 </param>
        /// <param name="length">读取长度</param>
        /// <returns>字符串</returns>
        public static string ReadString(byte[] message, ref int offset, int length)
        {
            if (length == 0) return string.Empty;
            var reault = DefaultEcoding.GetString(message, offset, length);
            offset += length;
            return reault.Trim(new[] { '\0' });
        }

        /// <summary>
        /// 使用base64编码读取数组
        /// </summary>
        /// <param name="message"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string Base64String(byte[] message, ref int offset, int length)
        {
            if (length == 0) return string.Empty;
            //var reault = DefaultEcoding.IsSingleByte`(message, offset, length);
            var uuid = Convert.ToBase64String(message, offset, length);
            offset += length;
            return uuid;//reault.Trim(new[] { '\0' });
        }

        /// <summary>
        /// 使用Guid编码读取数组
        /// </summary>
        /// <param name="messBytes"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string UuidString(byte[] messBytes, ref int offset, int length)
        {
            if (length == 0) return string.Empty;
            byte[] buffer = new byte[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = messBytes[offset++];
            }
            var uuid = new Guid(buffer);
            return uuid.ToString("N");
        }

        /// <summary>
        /// 使用Guid编码读取数组
        /// </summary>
        /// <param name="messageBytes"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static string UuidString(byte[] messageBytes, ref int offset)
        {
            var buffer = new byte[16];
            Array.ConstrainedCopy(messageBytes, offset, buffer, 0, buffer.Length);
            offset += 16;
            return new Guid(buffer).ToString("N");
        }

        /// <summary>
        /// 使用Base64编码方式写入字符串
        /// </summary>
        /// <param name="message"></param>
        /// <param name="length"></param>
        /// <param name="buffer"></param>
        public static void WriteBase64String(string message, int length, ref List<byte> buffer)
        {
            byte[] bytes;
            if (!string.IsNullOrEmpty(message))
            {
                bytes = Convert.FromBase64String(message);
                if (bytes.Length != length)
                    Array.Resize(ref bytes, length);
            }
            else
            {
                bytes = new byte[length];
            }
            buffer.AddRange(bytes);
        }

        /// <summary>
        /// 使用Guid编码方式写入字符串
        /// </summary>
        /// <param name="message"></param>
        /// <param name="length"></param>
        /// <param name="buffer"></param>
        public static void WriteUuidstring(string message, int length, ref List<byte> buffer)
        {
            byte[] bytes;
            if (!string.IsNullOrEmpty(message))
            {
                var uuid = new Guid(message);
                bytes = uuid.ToByteArray();
                if (bytes.Length != length)
                    Array.Resize(ref bytes, length);
            }
            else
            {
                bytes = new byte[length];
            }
            buffer.AddRange(bytes);
        }
        /// <summary>
        /// 使用协议编码方式写入字符串
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="length">读取长度</param>
        /// <param name="buffer">写入的缓存</param>
        public static void WriteString(string message, int length, ref List<byte> buffer)
        {
            byte[] bytes;
            if (!string.IsNullOrEmpty(message))
            {
                bytes = DefaultEcoding.GetBytes(message);
                if (bytes.Length != length)
                    Array.Resize(ref bytes, length);
            }
            else
            {
                bytes = new byte[length];
            }
            buffer.AddRange(bytes);
        }
        #endregion

        #region 解析相关

        /// <summary>
        /// 填充协议的子项内容
        /// </summary>
        /// <param name="message">原字节实体</param>
        /// <param name="offset">偏移位置</param>
        /// <param name="protocol">被填充的协议实体</param>
        protected abstract void OnDecomposeBody(byte[] message, ref int offset, BaseEntity protocol);

        /// <summary>
        /// 替换头内容，并重新计算校验位
        /// </summary>
        /// <param name="message">原消息</param>
        /// <param name="protocol">待替换的实体</param>
        public static void ReplaceHeader(ref byte[] message, BaseEntity protocol)
        {
            var offset = 2;
            message[offset++] = protocol.Encryption;
            var DataUnitLength = protocol.DataUnitLength.EndianUInt16ToBytes();
            foreach (var item in DataUnitLength)
            {
                message[offset++] = item;
            }
            message[offset++] = protocol.Type;
            message[offset++] = protocol.CMD;
            var FunctionCode = protocol.FunctionCode.EndianUInt16ToBytes();
            foreach (var item in FunctionCode)
            {
                message[offset++] = item;
            }
            message[offset++] = protocol.ResponseSign;
            message[offset++] = protocol.FunctionStatus;
            var vinno = new List<byte>();
            WriteString(protocol.UniqueIdentity, 17, ref vinno);
            foreach (var oneByte in vinno)
            {
                message[offset++] = oneByte;
            }
            message[offset++] = protocol.ProductType;
            message[offset++] = protocol.CloudProductCode;
            var times = protocol.TimeStamp.EndianULongToBytes();//DataTypeConversion.ConvertStringToDateTime(protocol.TimeStamp.ToString()).ToBytes6();
            foreach (var item in times)
            {
                message[offset++] = item;
            }
            // 从功能定义开始,计算校验位
            ushort crc = CheckCRC16(message, 5, message.Length);

            message[offset] = Convert.ToByte(crc);

        }

        /// <summary>
        /// 解析包头，并解析包体时间数据部分
        /// </summary>
        /// <param name="message">原字节实体</param>
        /// <param name="offset">偏移位置</param>
        /// <param name="protocol">被填充的协议实体</param>
        public static void DecomposeHeader(byte[] message, ref int offset, BaseEntity protocol)
        {
            // 跳过协议包头
            //offset += 2;
            // 读取加密方式
            protocol.Encryption = message[offset++];
            // 读取数据区长度
            protocol.DataUnitLength = message.ReadBigEndianUInt16(ref offset);
            //读取功能定义type
            protocol.Type = message[offset++];
            //读取功能定义CMD
            protocol.CMD = message[offset++];
            //读取功能号
            protocol.FunctionCode = message.ReadBigEndianUInt16(ref offset);
            //读取应答标志
            protocol.ResponseSign = message[offset++];
            //读取功能版本
            protocol.FunctionStatus = message[offset++];
            // 读取vinno
            protocol.UniqueIdentity = ReadString(message, ref offset, 17);
            //读取车机端产品类型
            protocol.ProductType = message[offset++];
            //读取云端产品代号
            protocol.CloudProductCode = message[offset++];
            //读取数据包时间戳
            protocol.TimeStamp = message.ReadBigEndianUInt64(ref offset);
        }


        /// <summary>
        /// 解析包尾，用填充CRC校验位
        /// </summary>
        /// <param name="message">原字节实体</param>
        /// <param name="offset">偏移位置</param>
        /// <param name="protocol">被填充的协议实体</param>
        public static void OnDecomposeTail(byte[] message, ref int offset, BaseEntity protocol)
        {
            protocol.CheckCode = message.ReadBigEndianUInt16(ref offset);//[offset++];
        }

        protected abstract BaseEntity CreateEntity(byte[] message);

        /// <summary>
        /// 协议解析
        /// </summary>
        /// <param name="message">原字节实体</param>
        /// <param name="offset">数据偏移位置</param>
        public BaseEntity DecomposeMessage(byte[] message, ref int offset)
        {
            // 生成协议实体
            var entity = CreateEntity(message);
            DecomposeHeader(message, ref offset, entity);
            // 判断是否为应答，如果为应答数据则只读取时间部分
            //if (entity.DataUnitLength == 6)
            //{
            //    entity.IsResponse = true;
            //}
            //else // 否则读取报文剩下内容
            OnDecomposeBody(message, ref offset, entity);
            OnDecomposeTail(message, ref offset, entity);
            return entity;

        }

        #endregion

        #region 消息生成

        /// <summary>
        /// 创建包头,并读取包体时间部分
        /// </summary>
        /// <param name="protocol">协议实体</param>
        /// <returns>返回包尾</returns>
        public static List<byte> CreateMessageHeader(BaseEntity protocol)
        {
            // 包头
            var result = new List<byte> { 0xFE, 0xFE, protocol.Encryption };
            //数据区长度
            var DataUnitLength = protocol.DataUnitLength.EndianUInt16ToBytes();
            result.AddRange(DataUnitLength);
            result.Add(protocol.Type);
            result.Add(protocol.CMD);
            var FunctionCode = protocol.FunctionCode.EndianUInt16ToBytes();
            result.AddRange(FunctionCode);
            result.Add(protocol.ResponseSign);
            result.Add(protocol.FunctionStatus);
            // vinno
            WriteString(protocol.UniqueIdentity, 17, ref result);
            result.Add(protocol.ProductType);
            result.Add(protocol.CloudProductCode);
            // 数据时间
            var TimeStamp = protocol.TimeStamp.EndianULongToBytes();//DataTypeConversion.ConvertStringToDateTime(protocol.TimeStamp.ToString()).ToBytes6();
            result.AddRange(TimeStamp);
            return result;
        }

        /// <summary>
        /// 获得子项字节数组
        /// </summary>
        /// <param name="protocol">协议实体</param>
        /// <returns>返回字节数组</returns>
        public abstract List<byte> CreateMessgeBody(BaseEntity protocol);

        /// <summary>
        /// 创建消息信息
        /// </summary>
        /// <param name="protocol">协议实体</param>
        /// <returns>消息</returns>
        public byte[] CreateMessage(BaseEntity protocol)
        {
            var bodyBytes = new List<byte>();
            // 如果为应答实体则不用生成包体除去时间外的其他数据
            if (protocol.IsResponse)
            {
                protocol.DataUnitLength = 6;
            }
            else
            {
                // 创建包体
                bodyBytes = CreateMessgeBody(protocol);
                // 数据区长度
                protocol.DataUnitLength += (bodyBytes.Count)._Ushort();
            }
            // 创建包头
            var result = CreateMessageHeader(protocol);
            // 添加包体字节
            result.AddRange(bodyBytes);

            protocol.CheckCode = CheckCRC16(result.ToArray(), 5, result.Count);
            // 创建包尾
            result.AddRange(protocol.CheckCode.EndianUInt16ToBytes());
            return result.ToArray();
        }

        #endregion

        #region 数据库实体转换
        /// <summary>
        /// 数据库实体生成
        /// </summary>
        /// <param name="protocol"></param>
        /// <returns></returns>
        public abstract BYDQBaseEntity CreatEntity(BaseEntity protocol);

        public abstract BaseEntity ConvertEntity(BYDQBaseEntity entity);
        #endregion

        #region CRC16校验
        /// <summary>
        /// CRC查询表
        /// </summary>
        public static readonly ushort[] CRC16Table = new ushort[]
            {
                0x0000,0x1021,0x2042,0x3063,0x4084,0x50a5,0x60c6,0x70e7,
                0x8108,0x9129,0xa14a,0xb16b,0xc18c,0xd1ad,0xe1ce,0xf1ef,
                0x1231,0x0210,0x3273,0x2252,0x52b5,0x4294,0x72f7,0x62d6,
                0x9339,0x8318,0xb37b,0xa35a,0xd3bd,0xc39c,0xf3ff,0xe3de,
                0x2462,0x3443,0x0420,0x1401,0x64e6,0x74c7,0x44a4,0x5485,
                0xa56a,0xb54b,0x8528,0x9509,0xe5ee,0xf5cf,0xc5ac,0xd58d,
                0x3653,0x2672,0x1611,0x0630,0x76d7,0x66f6,0x5695,0x46b4,
                0xb75b,0xa77a,0x9719,0x8738,0xf7df,0xe7fe,0xd79d,0xc7bc,
                0x48c4,0x58e5,0x6886,0x78a7,0x0840,0x1861,0x2802,0x3823,
                0xc9cc,0xd9ed,0xe98e,0xf9af,0x8948,0x9969,0xa90a,0xb92b,
                0x5af5,0x4ad4,0x7ab7,0x6a96,0x1a71,0x0a50,0x3a33,0x2a12,
                0xdbfd,0xcbdc,0xfbbf,0xeb9e,0x9b79,0x8b58,0xbb3b,0xab1a,
                0x6ca6,0x7c87,0x4ce4,0x5cc5,0x2c22,0x3c03,0x0c60,0x1c41,
                0xedae,0xfd8f,0xcdec,0xddcd,0xad2a,0xbd0b,0x8d68,0x9d49,
                0x7e97,0x6eb6,0x5ed5,0x4ef4,0x3e13,0x2e32,0x1e51,0x0e70,
                0xff9f,0xefbe,0xdfdd,0xcffc,0xbf1b,0xaf3a,0x9f59,0x8f78,
                0x9188,0x81a9,0xb1ca,0xa1eb,0xd10c,0xc12d,0xf14e,0xe16f,
                0x1080,0x00a1,0x30c2,0x20e3,0x5004,0x4025,0x7046,0x6067,
                0x83b9,0x9398,0xa3fb,0xb3da,0xc33d,0xd31c,0xe37f,0xf35e,
                0x02b1,0x1290,0x22f3,0x32d2,0x4235,0x5214,0x6277,0x7256,
                0xb5ea,0xa5cb,0x95a8,0x8589,0xf56e,0xe54f,0xd52c,0xc50d,
                0x34e2,0x24c3,0x14a0,0x0481,0x7466,0x6447,0x5424,0x4405,
                0xa7db,0xb7fa,0x8799,0x97b8,0xe75f,0xf77e,0xc71d,0xd73c,
                0x26d3,0x36f2,0x0691,0x16b0,0x6657,0x7676,0x4615,0x5634,
                0xd94c,0xc96d,0xf90e,0xe92f,0x99c8,0x89e9,0xb98a,0xa9ab,
                0x5844,0x4865,0x7806,0x6827,0x18c0,0x08e1,0x3882,0x28a3,
                0xcb7d,0xdb5c,0xeb3f,0xfb1e,0x8bf9,0x9bd8,0xabbb,0xbb9a,
                0x4a75,0x5a54,0x6a37,0x7a16,0x0af1,0x1ad0,0x2ab3,0x3a92,
                0xfd2e,0xed0f,0xdd6c,0xcd4d,0xbdaa,0xad8b,0x9de8,0x8dc9,
                0x7c26,0x6c07,0x5c64,0x4c45,0x3ca2,0x2c83,0x1ce0,0x0cc1,
                0xef1f,0xff3e,0xcf5d,0xdf7c,0xaf9b,0xbfba,0x8fd9,0x9ff8,
                0x6e17,0x7e36,0x4e55,0x5e74,0x2e93,0x3eb2,0x0ed1,0x1ef0
            };
        /// <summary>
        /// CRC16校验
        /// </summary>
        /// <param name="buffer">原数组</param>
        /// <param name="start">起始位置</param>
        /// <param name="bufferLength">计算长度</param>
        /// <returns>CRC校验结果</returns>
        public static ushort CheckCRC16(byte[] buffer, int start, int bufferLength)
        {
            ushort crc = 0xffff;
            for (var i = start; i < bufferLength; i++)
            {
                crc = (ushort)(((crc << 8) & 0xffff) ^
                                CRC16Table[((crc >> 8) ^ buffer[i])]);
            }

            return crc;
        }
        #endregion

        #region 消息ID

        /// <summary>
        /// 解析器标记，与所解析的实体标记对应
        /// </summary>
        public UInt32 ParserID { get; set; }

        #endregion
    }
}
