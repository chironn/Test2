/* ==============================================================================
* 类型名称：IFixBufferEntity  
* 类型描述：固定字节缓存通信数据实体接口
* 创 建 者：linfk
* 创建日期：2018/3/20 9:06:41
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

namespace DES.Core.Interfaces
{
    /// <summary>
    /// 固定字节缓存通信数据实体接口
    /// </summary>
    public interface IFixBufferEntity : ICommunicationEntity
    {

        /// <summary>
        /// 字节缓存
        /// </summary>
        byte[] Buffer { get; }

        /// <summary>
        /// 有效字节缓存偏移位置
        /// </summary>
        int Offset { get; }


        /// <summary>
        /// 有效字节长度
        /// </summary>
        int Length { get; set; }


        /// <summary>
        /// 最大使用字节长度
        /// </summary>
        int MaxSize { get; }

    }
}
