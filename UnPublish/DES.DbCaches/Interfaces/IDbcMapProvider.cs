/* ==============================================================================
 * 类型名称：DbcMapProvider  
 * 类型描述：
 * 创 建 者：linfk
 * 创建日期：2017/11/30 15:22:24
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/

using System.Collections.Generic;
using DES.DbCaches.Entities;
using System;

namespace DES.DbCaches.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDbcMapProvider
    {
        /// <summary>
        /// 异步操作过程中的异常操作
        /// </summary>
        event Action<Exception, string, ExchangeProtocolEntity> ExceptionEvent;

        /// <summary>
        /// 过程通知消息
        /// </summary>
        event Action<string, ExchangeProtocolEntity> NotifyEvent;

        /// <summary>
        /// 警告消息
        /// </summary>
        event Action<string, ExchangeProtocolEntity> WarnEvent;

        /// <summary>
        /// 错误事件
        /// </summary>
        event Action<string, ExchangeProtocolEntity> ErrorEvent;

        /// <summary>
        /// 根据数据项名称，计算DBC映射关系值
        /// </summary>
        /// <param name="propertyName">数据项名称</param>
        /// <param name="data">原始数据项</param>
        /// <param name="dbcmap">dbc映射关系</param>
        /// <returns>数据值</returns>
        decimal? FindValueFromDbc(string propertyName, ExchangeProtocolEntity data, ProtocolMapConfigEnttity dbcmap);

        /// <summary>
        /// 查询一组数据项值
        /// </summary>
        /// <param name="propertyName">数据项父项名称</param>
        /// <param name="subPropertyName">数据项子项名称</param>
        /// <param name="data">原始数据项</param>
        /// <param name="dbcmap">dbc映射关系</param>
        /// <param name="valueDic">返回结果</param>
        void FindValueFromDbc(string propertyName, List<string> subPropertyName, ExchangeProtocolEntity data,
                              ProtocolMapConfigEnttity dbcmap, out List<Dictionary<string, decimal?>> valueDic);

        /// <summary>
        /// 查询数据项列表值
        /// </summary>
        /// <param name="propertyName">数据项父项名称</param>
        /// <param name="subPropertyName">子项值</param>
        /// <param name="data">原始数据项</param>
        /// <param name="dbcmap">dbc映射关系</param>
        /// <param name="valueList">返回结果</param>
        void FindValueFromDbc(string propertyName, string subPropertyName, ExchangeProtocolEntity data,
                              ProtocolMapConfigEnttity dbcmap, out List<decimal?> valueList);


        /// <summary>
        /// 查询数据项列表值
        /// </summary>
        /// <param name="propertyName">数据项名称</param>
        /// <param name="data">原始数据项</param>
        /// <param name="dbcmap">dbc映射关系</param>
        /// <param name="valueList">返回结果</param>
        void FindValueFromDbc(string propertyName, ExchangeProtocolEntity data, ProtocolMapConfigEnttity dbcmap,
                                    out List<decimal?> valueList);
        /// <summary>
        /// 检查是否存在该数据项
        /// </summary>
        /// <param name="propertyName">数据项名称</param>
        /// <param name="data">原始数据项</param>
        /// <param name="dbcmap">dbc映射关系</param>
        /// <returns>存在返回true 不存在返回false</returns>
        bool CheckDbcMap(string propertyName, ExchangeProtocolEntity data, ProtocolMapConfigEnttity dbcmap);

        /// <summary>
        /// 检查是否存在该数据项
        /// </summary>
        /// <param name="propertyName">数据项名称</param>
        /// <param name="data">原始数据项</param>
        /// <param name="dbcmap">dbc映射关系</param>
        /// <returns>存在返回true 不存在返回false</returns>
        bool CheckDbcMapParent(string propertyName, ExchangeProtocolEntity data, ProtocolMapConfigEnttity dbcmap);

        /// <summary>
        /// 计算DBC映射关系值
        /// </summary>
        /// <param name="data">原始数据项</param>
        /// <param name="config">dbc映射关系</param>
        /// <returns>数据值</returns>
        decimal? FindValueFromDbc(ExchangeProtocolEntity data, ProtocolMapConfigEnttity config);

    }
}
