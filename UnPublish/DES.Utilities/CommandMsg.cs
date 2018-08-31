/* ==============================================================================
* 类型名称：CommandMsg  
* 类型描述：通用通信实体
* 创 建 者：linfk
* 创建日期：2017/11/29 11:12:50
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System;
using System.Collections.Generic;

namespace DES.Utilities
{
    /// <summary>
    /// 通用通信实体
    /// </summary>
    [ProtoBuf.ProtoContract]
    public class CommandMsg
    {

        public CommandMsg()
        {
            Content = new Dictionary<string, string>();
            EXTB = new Dictionary<string, decimal>();
            VIN = string.Empty;
            PR = string.Empty;
            TOKEN = string.Empty;
        }

        /// <summary>
        /// 键（终端号/车架号）
        /// </summary>
        [ProtoBuf.ProtoMember(1)]
        public string DK { get; set; }


        /// <summary>
        /// 协议版本号
        /// </summary>
        [ProtoBuf.ProtoMember(2)]
        public int VS { get; set; }
        /// <summary>
        /// 数据时间，时间格式“yyyy-MM-dd HH:mm:ss”
        /// </summary>
        [ProtoBuf.ProtoMember(3)]
        public DateTime RE { get; set; }

        /// <summary>
        /// 终端号
        /// </summary>
        [ProtoBuf.ProtoMember(4)]
        public uint TC { get; set; }

        /// <summary>
        /// 车架号
        /// </summary>
        [ProtoBuf.ProtoMember(5)]
        public string VIN { get; set; }

        /// <summary>
        /// 标识
        /// </summary>
        [ProtoBuf.ProtoMember(6)]
        public string FLAG { get; set; }

        /// <summary>
        /// 本消息的标识
        /// 用来作为消息的识别
        /// </summary>
        [ProtoBuf.ProtoMember(7)]
        public string TOKEN { get; set; }

        /// <summary>
        /// 通讯协议标识	"历史数据传输使用：History"
        /// </summary>
        [ProtoBuf.ProtoMember(8)]
        public string PR { get; set; }


        /// <summary>
        /// 传输数据内容
        /// </summary>
        [ProtoBuf.ProtoMember(9)]
        public Dictionary<string, string> Content { get; set; }


        /// <summary>
        /// 扩展字段Key为String 值为 double，存放经纬度数据
        /// </summary>
        [ProtoBuf.ProtoMember(9992)]
        public Dictionary<string, decimal> EXTB { get; set; }

        /// <summary>
        /// 是否为应答消息
        /// </summary>
        [ProtoBuf.ProtoMember(13)]
        public bool IsResponse { get; set; }

        /// <summary>
        /// 字节数据
        /// </summary>
        [ProtoBuf.ProtoMember(14)]
        public byte[] Bytes { get; set; }
        /// <summary>
        /// 数据项
        /// </summary>
        [ProtoBuf.ProtoMember(15)]
        public decimal?[] Data { get; set; }


        /// <summary>
        /// 数据接收时间，
        /// </summary>
        [ProtoBuf.ProtoMember(16)]
        public DateTime RCT { get; set; }


        /// <summary>
        /// 任务id
        /// </summary>
        [ProtoBuf.ProtoMember(17)]
        public int TKID { get; set; }

    }
}
