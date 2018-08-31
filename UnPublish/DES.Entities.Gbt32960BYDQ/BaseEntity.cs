using System;
using ProtoBuf.Meta;

namespace DES.Entities.BYDQ
{
    [ProtoBuf.ProtoContract]
    public class BaseEntity
    {

        public static void RegisteredProtoBuf()
        {
            RuntimeTypeModel protomodel = RuntimeTypeModel.Default;
            //为了避免和父类中的属性的定义的索引重复，这里对子类的索引可以定义的值较大一些
            protomodel[typeof(BaseEntity)].AddSubType(20, typeof(DES.Entities.BYDQ.DataMonitor));
            protomodel[typeof(BaseEntity)].AddSubType(40, typeof(DES.Entities.BYDQ.EventTrigger));
            protomodel[typeof(BaseEntity)].AddSubType(60, typeof(DES.Entities.BYDQ.DispatchInstructionDown));
            protomodel[typeof(BaseEntity)].AddSubType(80, typeof(DES.Entities.BYDQ.RemoteDebugDown));
            protomodel[typeof(BaseEntity)].AddSubType(100, typeof(DES.Entities.BYDQ.RemoteDebugUP));
        }


        public BaseEntity()
        {
            // 默认不加密
            Encryption = 0x00;
            // 功能版本 0x00
            FunctionStatus = 0x00;
            //默认数据区长度，功能定义，标识
            DataUnitLength = 35;
        }


        /// <summary>
        /// 是否为应答实体
        /// </summary>
        public bool IsResponse { get; set; }

        /// <summary>
        /// 加密方式 0x01：数据不加密；0x02：数据经过RSA算法加密；0x03:数据经过AES128位算法加密；“0xFE”表示异常，“0xFF”表示无效，其他预留
        /// </summary>
        [ProtoBuf.ProtoMember(1)]
        public byte Encryption { get; set; }

        /// <summary>
        /// 数据区长度
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public ushort DataUnitLength { get; set; }

        /// <summary>
        /// 功能定义TYPE
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public byte Type { get; set; }

        /// <summary>
        /// 功能定义CMD
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public byte CMD { get; set; }

        /// <summary>
        /// 功能号
        /// </summary>
        [ProtoBuf.ProtoMember(5)]
        public ushort FunctionCode { get; set; }

        /// <summary>
        /// 应答标志
        /// </summary>
        [ProtoBuf.ProtoMember(6)]
        public byte ResponseSign { get; set; }

        /// <summary>
        /// 功能版本
        /// </summary>
        [ProtoBuf.ProtoMember(7)]
        public byte FunctionStatus { get; set; }

        /// <summary>
        /// 整车VIN码 17位长度，不够补0。对于传输车辆数据时，此值为VIN码；对于传输其他数据时，此值为唯一标示
        /// </summary>
        [ProtoBuf.ProtoMember(8)]
        public string UniqueIdentity { get; set; }

        /// <summary>
        /// 车机端产品类型
        /// </summary>
        [ProtoBuf.ProtoMember(9)]
        public byte ProductType { get; set; }

        /// <summary>
        /// 云端产品代号 1:云服务1.0 2：云服务2.0
        /// </summary>
        [ProtoBuf.ProtoMember(10)]
        public byte CloudProductCode { get; set; }

        /// <summary>
        /// 数据包时间戳
        /// </summary>
        [ProtoBuf.ProtoMember(11)]
        public ulong TimeStamp { get; set; }

        /// <summary>
        /// 校验码
        /// </summary>
        [ProtoBuf.ProtoMember(12)]
        public ushort CheckCode { get; set; }

    }
}
