/* ==============================================================================
* 类型名称：ProtoBufSerialize  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/29 11:48:49
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System.IO;

namespace DES.Utilities.Serializies
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProtoBufSerialize
    {
        public static byte[] Serialize<TEntity>(TEntity entity)
        {
            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, entity);
                return stream.ToArray();
            }
        }

        public static TEntity Deserialize<TEntity>(byte[] buffer, int start, int length)
        {
            using (var stream = new MemoryStream(buffer, start, length))
            {
                return ProtoBuf.Serializer.Deserialize<TEntity>(stream);
            }
        }
    }
}
