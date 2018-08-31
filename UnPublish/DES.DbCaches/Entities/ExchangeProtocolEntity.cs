/* ==============================================================================
 * 类型名称：ExchangeProtocolEntity  
 * 类型描述：
 * 创 建 者：linfk
 * 创建日期：2017/11/30 15:27:27
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/

using System;

namespace DES.DbCaches.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ExchangeProtocolEntity
    {
        /// <summary>
        /// 解析incom报文数据时间
        /// </summary>
        public DateTime PackageFromInComTime { get; set; }

        /// <summary>
        /// 消息类型
        /// 0.表示实时数据
        /// 1.表示异常数据
        /// 0x10.标识终端鉴权信息
        /// 32960 表示透传的国家协议数据信息
        /// </summary>
        public int DataMsgType { get; set; }

        /// <summary>
        /// Vin码
        /// </summary>
        public string Vin { get; set; }



        /// <summary>
        /// 终端号
        /// </summary>
        public uint TerminalCode { get; set; }



        /// <summary>
        /// 实时数据终端上报时间
        /// </summary>
        public DateTime TravelTime { get; set; }

        #region 状态数据


        /// <summary>
        /// 是否行驶
        /// </summary>
        public bool IsDriving { get; set; }


        /// <summary>
        /// 
        /// 充电 0x01,
        /// 放电 0x02,
        /// 充放电 0x03,
        /// 
        /// 无效 255,
        /// 
        /// /*** 当为无效时，要进入DBC变量中查找是否有充、放电状态 ***/
        /// 
        /// </summary>
        public byte ChargeOrDischarge { get; set; }

        /// <summary>
        /// GPS是否定位
        /// </summary>
        public byte IsGPSPosition { get; set; }

        /// <summary>
        /// Can产线是否有数据
        /// </summary>
        public byte IsCanHasData { get; set; }


        /// <summary>
        /// 目的地,目的地编号
        /// </summary>
        public byte Destination { get; set; }


        /// <summary>
        /// 车锁定状态
        /// </summary>
        public byte IsLock { get; set; }


        /// <summary>
        /// 车辆故障状态，１表示有故障，０表示无故障
        /// </summary>
        public byte IsVehicleHasFault { get; set; }

        /// <summary>
        /// 锁车装置状态
        /// </summary>
        public byte LockCarEquipmentState { get; set; }

        /// <summary>
        /// 终端状态
        /// </summary>
        public byte TerminalParameterState { get; set; }

        /// <summary>
        /// 是否缓存数据,0表示历史，1表示实时
        /// </summary>
        public byte IsCache { get; set; }


        /// <summary>
        /// 休眠模式,0表示未休眠
        /// </summary>
        public byte SleepMode { get; set; }



        #endregion


        #region GPS定位数据

        /// <summary>
        /// GPS是否有效,1表示有效，0表示无效
        /// </summary>
        public byte IsGpsEffective { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        public decimal Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public decimal Longitude { get; set; }

        /// <summary>
        /// 海拔高度 1 km
        /// </summary>
        public double Altitude { get; set; }

        /// <summary>
        /// GPS车速,1km/h
        /// </summary>
        public decimal GPSSpeed { get; set; }

        /// <summary>
        /// GPS方向,单位 0
        /// </summary>
        public decimal GPSDirection { get; set; }

        /// <summary>
        /// GPS里程,单位 0.1KM
        /// </summary>
        public decimal GPSMileage { get; set; }


        #endregion

        #region 自定义DBC变量数据

        /// <summary>
        /// 变量总数
        /// </summary>
        public int DbcVariablesCount { get; set; }

        /// <summary>
        /// Md5值
        /// </summary>
        public string Md5Code { get; set; }

        /// <summary>
        /// Dbc变量数据个数
        /// </summary>
        public decimal?[] VariablesData { get; set; }

        #endregion
    }
}
