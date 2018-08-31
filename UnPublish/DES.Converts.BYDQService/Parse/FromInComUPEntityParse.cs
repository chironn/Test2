using System;
using DES.Core;
using DES.Core.Interfaces;
using Exchange.Framwork.Entities;
using DES.Protocols.BYDQ;
using DES.Entities.BYDQ;
using DES.Protocols.BYDQ.Extends;
using DES.Converts.BYDQService.Entity;

namespace DES.Converts.BYDQService.Parse
{
    /// <summary>
    ///  车机到云端
    /// </summary>
    public class FromInComUPEntityParse : ToLogic<FromInComEntity, IFixBufferEntity>
    {
        private readonly ParserManager _parserManger = new ParserManager();

        public FromInComUPEntityParse()
        {
            //手动初始化实时数据池
            ExchangeProtocolEntityPool.Create().Initialize(100, 800);
        }

        #region Overrides of ToLogic<FromInComEntity,IFixBufferEntity>

        public override string FindKey
        {
            get { return "BYD企标上行数据接收队列"; }
        }

        #region 协议解析
        /// <summary>
        /// 3006,CAN数据多个ID混合组帧转发
        /// </summary>
        /// <param name="parse"></param>
        /// <param name="caches"></param>
        /// <returns></returns>
        private FromInComEntity DataMonitor(IFixBufferEntity caches)
        {
            var buffer = new byte[caches.Length];
            Array.Copy(caches.Buffer, caches.Offset, buffer, 0, buffer.Length);
            int offset = 2;
            var real = (DataMonitor)_parserManger.Decompose(buffer, ref offset);
            if (real == null) return null;

            return new FromInComEntity
                {
                    FindKey = FindKey,
                    RealEntity = real
                };
        }

        /// <summary>
        /// 3007,事件触发描述信息上传
        /// </summary>
        /// <param name="caches"></param>
        /// <returns></returns>
        private FromInComEntity EventTrigger(IFixBufferEntity caches)
        {
            var buffer = new byte[caches.Length];
            Array.Copy(caches.Buffer, caches.Offset, buffer, 0, buffer.Length);
            int offset = 2;
            var real = (EventTrigger)_parserManger.Decompose(buffer, ref offset);
            if (real == null) return null;

            return new FromInComEntity
            {
                FindKey = FindKey,
                RealEntity = real
            };
        }
        /// <summary>
        /// 7002,CAN数据诊断
        /// </summary>
        /// <param name="parse"></param>
        /// <param name="caches"></param>
        /// <returns></returns>
        private FromInComEntity RemoteDebugUP(IFixBufferEntity caches)
        {
            var buffer = new byte[caches.Length];
            Array.Copy(caches.Buffer, caches.Offset, buffer, 0, buffer.Length);
            int offset = 2;
            var real = (RemoteDebugUP)_parserManger.Decompose(buffer, ref offset);
            if (real == null) return null;

            return new FromInComEntity
            {
                FindKey = FindKey,
                RealEntity = real
            };
        }
        #endregion
        protected override FromInComEntity OnConvert(IFixBufferEntity communicationEntity)
        {
            //协议号 索引1开始,取7字节
            int offset = communicationEntity.Offset + 7;
            var key = communicationEntity.Buffer.ReadBigEndianUInt16(ref offset);
            
            //获取解析字典
            switch (key)
            {
                //CAN数据多个ID混合组帧转发
                case 3006:
                    return DataMonitor(communicationEntity);
                //事件触发描述信息上传
                case 3007:
                    return EventTrigger(communicationEntity);
                //CAN数据诊断
                case 7002:
                    return RemoteDebugUP(communicationEntity);
                default:
                    break;
            }
            return null;
        }

        #endregion

    }
}
