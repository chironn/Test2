/* ==============================================================================
* 类型名称：BufferEntity  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/19 19:22:48
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using DES.Core.Interfaces;

namespace DES.Core
{
    /// <summary>
    ///  通信字节实体
    /// </summary>
    public abstract class BufferEntity : IFixBufferEntity
    {
        public string FindKey { get; set; }

        public byte[] Bytes;

        /// <summary>
        /// 数据字节长度
        /// </summary>
        public int BytesLength { get; set; }

        public abstract void SetRouteing(string routeing);

        public abstract string GetRoteing();

        #region Implementation of IFixBufferEntity

        /// <summary>
        /// 字节缓存
        /// </summary>
        public byte[] Buffer { get { return Bytes; } }

        /// <summary>
        /// 有效字节缓存偏移位置
        /// </summary>
        public int Offset { get { return 0; } }

        /// <summary>
        /// 有效字节长度
        /// </summary>
        public int Length
        {
            get { return BytesLength; }
            set { BytesLength = value; }
        }

        /// <summary>
        /// 最大使用字节长度
        /// </summary>
        public int MaxSize
        {
            get
            {
                return Bytes == null ? 0 :
                    Bytes.Length;
            }
        }

        #endregion
    }
}
