using DES.Core;
using System;
using DES.Converts.BYDQService.Entity;

namespace DES.Converts.BYDQService.Protocol
{
    public class ProtocolParseDownload : ToCommunication<BufferEntity, BaseLogicEntity<FromIncomBufferEntity>>
    {

        public override string FindKey
        {
            get { return "BYD企标下行数据发布队列"; }
        }

        protected override void OnConvert(BufferEntity cEntity, BaseLogicEntity<FromIncomBufferEntity> lEntity)
        {
            // 转换字节数据
            var msg = OnConvert(lEntity.Entity);
            //var caches = ProtoBufSerialize.Serialize(msg);
            // 创建通信实体
            InitilizeBuffer(cEntity, msg);
        }

        protected override void SetRouteing(BufferEntity cEntity, BaseLogicEntity<FromIncomBufferEntity> lEntity)
        {
            var routeingString = GetRouteingString(lEntity.Entity);
            cEntity.SetRouteing(routeingString);
        }

        private string GetRouteingString(FromIncomBufferEntity fromIncomBufferEntity)
        {
            return string.Format(",{0}", fromIncomBufferEntity.RoutingKey);
        }

        protected byte[] OnConvert(FromIncomBufferEntity entity)
        {
            return entity.Buffer;
        }

        protected void InitilizeBuffer(BufferEntity buffer, byte[] caches)
        {

            if (buffer == null) return;

            buffer.FindKey = FindKey;


            if (buffer.Bytes == null)
            {
                buffer.Bytes = new byte[caches.Length];
            }
            else if (buffer.Bytes.Length < caches.Length)
            {
                Array.Resize(ref buffer.Bytes, caches.Length);
            }
            Array.Copy(caches, 0, buffer.Bytes, 0, caches.Length);

            buffer.BytesLength = caches.Length;

        }
    }
}
