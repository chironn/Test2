/* ==============================================================================
* 类型名称：BaseToCommunication  
* 类型描述：通信实体转换基类
* 创 建 者：linfk
* 创建日期：2017/11/29 16:37:51
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System;
using DES.Core.Interfaces;
using DES.Utilities;
using DES.Utilities.Serializies;

namespace DES.Core
{

    public abstract class ToCommunication<TCEntity, TLEntity> : IToCommunication
        where TLEntity : class, ILogicEntity
        where TCEntity : class, ICommunicationEntity
    {

        public abstract string FindKey { get; }

        public BaseService Service { get; set; }

        protected abstract void OnConvert(TCEntity cEntity, TLEntity lEntity);

        protected abstract void SetRouteing(TCEntity cEntity, TLEntity lEntity);

        public void Convert(ICommunicationEntity communicationEntity, ILogicEntity entity)
        {
            try
            {
                var cEntity = communicationEntity as TCEntity;
                OnConvert(cEntity, entity as TLEntity);
                SetRouteing(cEntity, entity as TLEntity);
            }
            finally
            {
                if (Service != null && Service.CommunicateFactory != null)
                {
                    Service.CommunicateFactory.GivebackEntity(communicationEntity);
                }
            }
        }
    }




    /// <summary>
    /// 通信实体转换基类
    /// </summary>
    public abstract class BaseLogicEntityToBufferEntity<TEntity> : ToCommunication<BufferEntity, BaseLogicEntity<TEntity>>
    {

        protected abstract CommandMsg OnConvert(TEntity entity);

        protected abstract string GetRouteingString(TEntity entity);

        protected override void SetRouteing(BufferEntity cEntity, BaseLogicEntity<TEntity> lEntity)
        {
            var routeingString = GetRouteingString(lEntity.Entity);
            cEntity.SetRouteing(routeingString);
        }


        protected virtual void InitilizeBuffer(BufferEntity buffer, byte[] caches)
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

        protected override void OnConvert(BufferEntity cEntity, BaseLogicEntity<TEntity> lEntity)
        {
            // 转换字节数据
            var msg = OnConvert(lEntity.Entity);
            var caches = ProtoBufSerialize.Serialize(msg);
            // 创建通信实体
            InitilizeBuffer(cEntity, caches);
        }
    }
}
