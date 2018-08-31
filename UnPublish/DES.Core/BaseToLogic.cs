/* ==============================================================================
* 类型名称：BaseToLogic  
* 类型描述：基础逻辑类
* 创 建 者：linfk
* 创建日期：2017/11/29 11:52:09
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using DES.Core.Interfaces;
using DES.Utilities;
using DES.Utilities.Serializies;

namespace DES.Core
{

    public abstract class ToLogic<TLEntity, TCEntity> : IToLogic
        where TLEntity : class, ILogicEntity
        where TCEntity : class, ICommunicationEntity
    {

        public abstract string FindKey { get; }

        public BaseService Service { get; set; }

        protected abstract TLEntity OnConvert(TCEntity communicationEntity);

        public ILogicEntity Convert(ICommunicationEntity entity)
        {
            try
            {
                return OnConvert(entity as TCEntity);
            }
            finally
            {
                // 数据还池
                if (Service != null)
                {
                    Service.CommunicateFactory.GivebackEntity(entity);
                }
            }
        }
    }

    /// <summary>
    /// 基础逻辑类
    /// </summary>
    public abstract class AnyToLogic<TEntity> : IToLogic
    {
        public abstract string FindKey { get; }

        public BaseService Service { get; set; }

        protected abstract BaseLogicEntity<TEntity> OnConvert(CommandMsg entity);

        protected virtual BaseLogicEntity<TEntity> OnConvert(IFixBufferEntity entity)
        {
            var msg = ProtoBufSerialize.Deserialize<CommandMsg>(entity.Buffer, entity.Offset, entity.Length);
            var logicEntity = OnConvert(msg);
            logicEntity.FindKey = FindKey;
            return logicEntity;
        }

        public ILogicEntity Convert(ICommunicationEntity entity)
        {
            try
            {
                return OnConvert(entity as IFixBufferEntity);
            }
            finally
            {
                // 数据还池
                if (Service != null)
                {
                    Service.CommunicateFactory.GivebackEntity(entity);
                }
            }
        }
    }

    /// <summary>
    /// 基础逻辑类
    /// </summary>
    public class MsgToLogic : AnyToLogic<CommandMsg>
    {
        protected string LocalFindKey = "default";
        public override string FindKey { get { return LocalFindKey; } }

        protected override BaseLogicEntity<CommandMsg> OnConvert(CommandMsg entity)
        {
            return new BaseLogicEntity<CommandMsg> { Entity = entity };
        }

        protected override BaseLogicEntity<CommandMsg> OnConvert(IFixBufferEntity entity)
        {
            var result = base.OnConvert(entity);
            result.FindKey = entity.FindKey;
            return result;
        }
    }
}
