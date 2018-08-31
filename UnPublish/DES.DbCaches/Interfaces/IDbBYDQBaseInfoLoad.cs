using System;
using System.Collections.Generic;
using DES.DbCaches.DbEntities;

namespace DES.DbCaches.Interfaces
{
    public interface IDbBYDQBaseInfoLoad
    {
        /// <summary>
        /// 记录错误日志操作
        /// </summary>
        event Action<string> WarnEvent;
        /// <summary>
        /// 记录耗时记录操作
        /// </summary>
        event Action<string> InfoEvent;

        /// <summary>
        /// 更新7002下行指令Json数据段
        /// </summary>
        /// <param name="jsonResultEntity"></param>
        /// <returns></returns>
        bool UpdateRemoteDebugUpBaseInfos(List<RemoteDebugUPJsonResultEntity> jsonResultEntity);

        /// <summary>
        /// 更新7002下行指令数据段
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        bool UpdateeRemoteDebugData(byte[] buffer);

        /// <summary>
        /// 将BYDQ协议3006数据入库
        /// </summary>
        /// <returns></returns>
        bool SaveDataMonitorBaseInfos(List<DataMonitorEntity> trans);

        /// <summary>
        /// 将BYDQ协议3007数据入库
        /// </summary>
        /// <param name="trans"></param>
        /// <returns></returns>
        bool SaveEventTriggerBaseInfos(List<EventTriggerEntity> trans);

        /// <summary>
        /// 将BYDQ协议7002数据入库
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool SaveRemoteDebugUPBaseInfos(List<RemoteDebugUPEntity> trans);

        /// <summary>
        /// 获得UUID与文件路径
        /// </summary>
        /// <returns></returns>
        List<DispatchInstructionDownEntity> GetOrderList(string state);

        /// <summary>
        /// 获得UDS指令的UUID与文件路径
        /// </summary>
        /// <returns></returns>
        List<RemoteDebugDownEntity> GetUDSOderList(string state);

        /// <summary>
        /// 获取3006功能类型数据
        /// </summary>
        /// <param name="UUID">下发指令的UUID</param>
        /// <returns></returns>
        List<DataMonitorEntity> GetDataMonitorDataList(string UUID);

        /// <summary>
        /// 分页查询7002功能类型数据
        /// </summary>
        /// <param name="startNum"></param>
        /// <param name="endNum"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<RemoteDebugUPEntity> GetPageRemoteDebugDataList(string startNum, string endNum, 
            string condition);

        /// <summary>
        /// 分页查询3006功能类型数据
        /// </summary>
        /// <param name="startNum"></param>
        /// <param name="endNum"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        List<DataMonitorEntity> GetPageDataMonitorDataList(string startNum, string endNum,
            string condition);

        /// <summary>
        /// 更新3006类型数据下载状态
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="state"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        bool UpdateMonitorDataState(string uuid, string state, string errorMsg,string Path);

        /// <summary>
        /// 更新7002类型数据下载状态
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="state"></param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        bool UpdateeRemoteDebugDataState(string uuid, string state, string errorMsg);
    }
}
