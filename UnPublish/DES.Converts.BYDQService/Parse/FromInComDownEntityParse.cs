using System;
using DES.Core;
using DES.Core.Interfaces;
using Exchange.Framwork.Entities;
using DES.Protocols.BYDQ;
using DES.Entities.BYDQ;
using DES.Utilities.Serializies;
using DES.Converts.BYDQService.Entity;

namespace DES.Converts.BYDQService.Parse
{
    /// <summary>
    /// 下行,云端到车机
    /// </summary>
    public class FromInComDownEntityParse : ToLogic<FromIncomBufferEntity, IFixBufferEntity>
    { 
        private readonly ParserManager _parserManger = new ParserManager();

        public FromInComDownEntityParse()
        {
            //手动初始化实时数据池
            ExchangeProtocolEntityPool.Create().Initialize(100, 800);
        }
        public override string FindKey
        {
            get { return "BYD企标下行数据接收队列"; }
        }
        
        /// <summary>
        /// 下行数据实体转buffer实体
        /// </summary>
        /// <param name="caches"></param>
        /// <returns></returns>
        private FromIncomBufferEntity BaseEntityToBuffer(BaseEntity caches)
        {
            
            var bufferSend = _parserManger.GetBytes(caches);
            return new FromIncomBufferEntity
            {
                FindKey = FindKey,
                RoutingKey = caches.FunctionCode,
                Buffer = bufferSend
            };
        }

        protected override FromIncomBufferEntity OnConvert(IFixBufferEntity communicationEntity)
        {
            try
            {
                var proEntity = ProtoBufSerialize.Deserialize<BaseEntity>(communicationEntity.Buffer, communicationEntity.Offset, communicationEntity.Length);
                //获取解析字典
                return BaseEntityToBuffer(proEntity);
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }
    }
}
