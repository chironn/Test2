using System;
using DES.Core;
using DES.Entities.BYDQ;
using DES.Utilities.Serializies;

namespace DES.Converts.BYDQService.Protocol
{
    public class ProtocolParseUpload : ToCommunication<BufferEntity, BaseLogicEntity<BaseEntity>>
    {

        public override string FindKey
        {
            get { return "BYD企标上行数据发布队列"; }
        }

        protected override void OnConvert(BufferEntity cEntity, BaseLogicEntity<BaseEntity> lEntity)
        {
            var msg = OnConvert(lEntity.Entity);//lEntity.Entity;
            var caches = ProtoBufSerialize.Serialize(msg);
            // 创建通信实体
            InitilizeBuffer(cEntity, caches);
        }

        protected override void SetRouteing(BufferEntity cEntity, BaseLogicEntity<BaseEntity> lEntity)
        {
            var routeingString = GetRouteingString(lEntity.Entity);
            cEntity.SetRouteing(routeingString);
        }

        private string GetRouteingString(BaseEntity fromIncomBufferEntity)
        {
            return string.Format(",{0}", fromIncomBufferEntity.FunctionCode);
        }
        protected BaseEntity OnConvert(BaseEntity entity)
        {
            return entity;
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
