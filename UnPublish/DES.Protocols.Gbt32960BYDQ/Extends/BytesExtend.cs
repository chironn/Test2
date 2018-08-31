using System;
using System.Globalization;
using System.Net;

namespace DES.Protocols.BYDQ.Extends
{
    /// <summary>
    /// 
    /// </summary>
    public static class BytesExtend
    {

        /// <summary>
        /// 读取两个字节的大端数字(ushort)，并自动转换为C#的小端值
        /// </summary>
        /// <param name="messge">消息缓存</param>
        /// <param name="offset">偏移</param>
        /// <returns>结果</returns>
        public static ushort ReadBigEndianUInt16(this byte[] messge, ref int offset)
        {
            var result = BitConverter.ToInt16(messge, offset);
            offset += 2;
            return (ushort)IPAddress.NetworkToHostOrder(result);
        }

        /// <summary>
        /// 读取八个字节的大端数字(ulong)，并自动转换为C#的小端值
        /// </summary>
        /// <param name="messge">消息缓存</param>
        /// <param name="offset">偏移</param>
        /// <returns>结果</returns>
        public static ulong ReadBigEndianUInt64(this byte[] messge, ref int offset)
        {
            var result = BitConverter.ToInt64(messge, offset);
            offset += 8;
            return (ulong)IPAddress.NetworkToHostOrder(result);
        }

        /// <summary>
        /// 将UInt16数值转换大端为字节数据
        /// </summary>
        /// <param name="number">本地字节数据</param>
        /// <returns>网络字节数据</returns>
        public static byte[] EndianUInt16ToBytes(this UInt16 number)
        {
            return BitConverter.GetBytes((ushort)(IPAddress.HostToNetworkOrder(number) >> 16));
        }
        /// <summary>
        /// 将ulong数值转换大端为字节数据
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static byte[] EndianULongToBytes(this ulong number)
        {
            byte[] b = BitConverter.GetBytes(number);
            Array.Reverse(b);
            return b;
            
            //byte[] b = new byte[8];
            //b[0] = (byte)(number & 0xFF);
            //b[1] = (byte)(number >> 8 & 0xFF);
            //b[2] = (byte)(number >> 16 & 0xFF);
            //b[3] = (byte)(number >> 24 & 0xFF);
            //b[4] = (byte)(number >> 32 & 0xFF);
            //b[5] = (byte)(number >> 40 & 0xFF);
            //b[6] = (byte)(number >> 48 & 0xFF);
            //b[7] = (byte)(number >> 56 & 0xFF);
            //return b;
        }
        /// <summary>
        /// 读取四个字节的大端数字，并自动转换为C#的小端值
        /// </summary>
        /// <param name="messge">消息缓存</param>
        /// <param name="offset">偏移</param>
        /// <returns>结果</returns>
        public static uint ReadBigEndianUInt32(this byte[] messge, ref int offset)
        {
            var result = BitConverter.ToInt32(messge, offset);
            offset += 4;
            return (uint)IPAddress.NetworkToHostOrder(result);
        }

        /// <summary>
        /// 将UInt32数值转换大端为字节数据
        /// </summary>
        /// <param name="number">本地字节数据</param>
        /// <returns>网络字节数据</returns>
        public static byte[] EndianUInt32ToBytes(this UInt32 number)
        {
            return BitConverter.GetBytes((uint)(IPAddress.HostToNetworkOrder(number) >> 32));
        }

        /// <summary>
        /// 读取字节字节数组中的日期数据读取失败返回时间默认值
        /// </summary>
        /// <param name="bytes6">字节数据</param>
        /// <param name="offset">起始偏移</param>
        /// <returns>成功返回读取结果，失败返回最小时间数据</returns>
        public static DateTime ReadBytes6Time(this byte[] bytes6, ref int offset)
        {
            try
            {
                var result = new DateTime(
                    2000 + bytes6[offset],
                    bytes6[offset + 1],
                    bytes6[offset + 2],
                    bytes6[offset + 3],
                    bytes6[offset + 4],
                    bytes6[offset + 5]);
                offset += 6;
                return result;
            }
            catch (Exception)
            {
                return default(DateTime);
            }
        }

        /// <summary>
        /// 将时间转换为6字节byte数据
        /// </summary>
        /// <param name="time">时间</param>
        /// <returns>返回6字节</returns>
        public static byte[] ToBytes6(this DateTime time)
        {
            var data = new[]
                {
                    ((byte) (time.Year%1000)),
                    ((byte) time.Month),
                    ((byte) time.Day),
                    ((byte) time.Hour),
                    ((byte) time.Minute),
                    ((byte) time.Second)
                };
            return data;
        }


        /// <summary>
        /// 从数组中读取一个int, double4字节
        /// </summary>
        /// <param name="message">数组</param>
        /// <param name="index">当前读取索引</param>
        /// <returns></returns>
        public static int ReadInt32(this byte[] message, ref int index)
        {
            int result = BitConverter.ToInt32(message, index);
            index += 4;
            return result;
        }

        /// <summary>
        /// 从数组中读取一个uint, 4字节
        /// </summary>
        /// <param name="message">数组</param>
        /// <param name="index">当前读取索引</param>
        /// <returns></returns>
        public static uint ReadUInt32(this byte[] message, ref int index)
        {
            var result = BitConverter.ToUInt32(message, index);
            index += 4;
            return result;
        }

        /// <summary>
        /// 从数组中读取一个Ushort, Ushort 2字节
        /// </summary>
        /// <param name="message">数组</param>sf
        /// <param name="index">当前读取索引</param>
        /// <returns></returns>
        public static ushort ReadUshort(this byte[] message, ref int index)
        {
            if (message == null) throw new ArgumentNullException("message");
            var result = BitConverter.ToUInt16(message, index);
            index += 2;
            return result;
        }
        /// <summary>
        /// 从数组中读取一个字节
        /// </summary>
        /// <param name="message">数组</param>
        /// <param name="index">当前读取索引</param>
        public static byte ReadByte(this byte[] message, ref int index)
        {
            byte result = message[index];
            index += 1;
            return result;
        }


        /// <summary>
        /// 将 int 类型数据强制转换为 byte 数据，若数据值不在最大值和最小值之间则返回边界值,例如：min=1,max=3 时,若 value=-1 则 返回 1 若 value=4 则返回3 若value=2 则返回2
        /// </summary>
        /// <param name="value">数据值</param>
        /// <param name="min">最小边界值</param>
        /// <param name="max">最大边界值</param>
        /// <returns>强转结果</returns>
        public static byte _Byte(this int value, byte min = byte.MinValue, byte max = byte.MaxValue)
        {
            if (value <= min)
                return min;
            if (value >= max)
                return max;
            return (byte)value;
        }


     

        /// <summary>
        /// 将 int 类型数据强制转换为 ushort 数据，若数据值不在最大值和最小值之间则返回边界值,例如：min=1,max=3 时,若 value=-1 则 返回 1 若 value=4 则返回3 若value=2 则返回2
        /// </summary>
        /// <param name="value">数据值</param>
        /// <param name="min">最小边界值</param>
        /// <param name="max">最大边界值</param>
        /// <returns>强转结果</returns>
        public static ushort _Ushort(this int value, ushort min = ushort.MinValue, ushort max = ushort.MaxValue)
        {
            if (value <= min)
                return min;
            if (value >= max)
                return max;
            return (ushort)value;
        }

        /// <summary>
        /// DateTime --> long
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static long ConvertDataTimeToLong(DateTime dt)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = dt.Subtract(dtStart);
            long timeStamp = toNow.Ticks;
            timeStamp = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 7));
            return timeStamp;
        }


        /// <summary>
        /// long --> DateTime
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static DateTime ConvertLongToDateTime(long d)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = 0;
            switch (d.ToString(CultureInfo.InvariantCulture).Length)
            {
                case 10:
                    lTime = long.Parse(d + "0000000");
                    break;
                case 13:
                    lTime = long.Parse(d + "0000");
                    break;
            }
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult;
        }
    }
}
