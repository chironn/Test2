/* ==============================================================================
* 类型名称：Expends  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/12/5 11:29:00
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DES.Utilities.Expends
{
    /// <summary>
    /// 
    /// </summary>
    public static class Expends
    {
        private readonly static System.Collections.Concurrent.BlockingCollection<byte[]> _bytesQueue =
            new System.Collections.Concurrent.BlockingCollection<byte[]>();

        /// <summary>
        /// 解压缩字节数据
        /// </summary>
        /// <param name="data">原字节数据</param>
        /// <returns>解压缩后字节数据</returns>
        public static byte[] Decompress(this byte[] data)
        {
            byte[] buffer;
            _bytesQueue.TryTake(out buffer, 100);
            buffer = buffer ?? new byte[3 * 1024];
            try
            {
                var ms = new MemoryStream(data);
                var zip = new GZipStream(ms, CompressionMode.Decompress, true);
                using (var msreader = new MemoryStream())
                {
                    while (true)
                    {
                        var reader = zip.Read(buffer, 0, buffer.Length);
                        if (reader <= 0)
                        {
                            break;
                        }
                        msreader.Write(buffer, 0, reader);
                    }
                    zip.Close();
                    ms.Close();
                    msreader.Position = 0;
                    return msreader.ToArray();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                _bytesQueue.TryAdd(buffer, 100);
            }
        }

        /// <summary>
        /// 压缩字节数据
        /// </summary>
        /// <param name="data">原字节数据</param>
        /// <returns>压缩后字节数据</returns>
        /// <returns></returns>
        public static byte[] Compress(this byte[] data)
        {
            try
            {
                var ms = new MemoryStream();
                var zip = new GZipStream(ms, CompressionMode.Compress, true);
                zip.Write(data, 0, data.Length);
                zip.Close();
                var buffer = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(buffer, 0, buffer.Length);
                ms.Close();
                return buffer;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// 压缩字符串数据
        /// </summary>
        /// <param name="str">字符串数据</param>
        /// <returns>压缩后的字节数据</returns>
        public static byte[] CompressString(this string str)
        {
            var compressBeforeByte = Encoding.GetEncoding("UTF-8").GetBytes(str);
            var compressAfterByte = compressBeforeByte.Compress();
            return compressAfterByte;
        }

        /// <summary>
        /// 解压缩字符串数据
        /// </summary>
        /// <param name="bytes">待解压的字节数据</param>
        /// <returns>解压后的字符串信息</returns>
        public static string DecompressString(this byte[] bytes)
        {
            var compressAfterByte = bytes.Decompress();
            return Encoding.GetEncoding("UTF-8").GetString(compressAfterByte);
        }

        /// <summary>
        /// 将字节数据转换为16进制字符串(release 版本比系统方法“BitConverter.ToString”然后再调用“Replace”方法高效一倍左右。
        /// </summary>
        /// <param name="value">字节数据</param>
        /// <param name="startIndex">转换的起始字节位置</param>
        /// <param name="length">转换的长度</param>
        /// <returns>16进制字符串，每两个字符标识一个字节</returns>
        public static string BytesToString(this byte[] value, int startIndex, int length)
        {
            // 字符长度是字节长度的2倍
            var charlength = (length - startIndex) * 2;
            var hexArrary = new char[charlength];
            int i;
            var index = startIndex;
            for (i = 0; i < charlength; i += 2)
            {
                var b = value[index++];
                hexArrary[i] = GetHexValue(b / 16);
                hexArrary[i + 1] = GetHexValue(b % 16);
            }

            return new string(hexArrary, 0, charlength);
        }

        public static string BytesToString(this byte[] value)
        {
            return BytesToString(value, 0, value.Length);
        }

        public static byte[] StringToBytes(this string value)
        {
            var hexArrary = value.ToCharArray();
            var charlength = hexArrary.Length / 2;
            var bytes = new byte[charlength];
            var index = 0;
            for (var i = 0; i < value.Length; i += 2)
            {
                var b = (byte)(GetValue((hexArrary[i])) * 16);
                b += GetValue(hexArrary[i + 1]);
                bytes[index++] = b;
            }

            return bytes;
        }

        /// <summary>
        /// 获取 0~16 范围内的字符对应的16进制字符 
        /// </summary>
        /// <param name="i">10进制数据</param>
        /// <returns>16进程字符</returns>
        public static char GetHexValue(int i)
        {
            if (i < 10)
            {
                return (char)(i + '0');
            }

            return (char)(i - 10 + 'A');
        }

        public static byte GetValue(char c)
        {
            if (c >= 'A')
            {
                return (byte)(c + 10 - 'A');
            }

            return (byte)(c - '0');
        }

    }
}
