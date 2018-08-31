/* ==============================================================================
* 类型名称：DbcMapProvider  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/12/1 13:14:43
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DES.DbCaches.Entities;
using DES.DbCaches.Interfaces;

namespace DES.DbCaches.Implementeds
{
    /// <summary>
    /// 
    /// </summary>
    public class DbcMapProvider : IDbcMapProvider
    {
        public event Action<Exception, string, ExchangeProtocolEntity> ExceptionEvent;

        public event Action<string, ExchangeProtocolEntity> NotifyEvent;

        public event Action<string, ExchangeProtocolEntity> WarnEvent;

        public event Action<string, ExchangeProtocolEntity> ErrorEvent;



        public decimal? FindValueFromDbc(string propertyName, ExchangeProtocolEntity data, ProtocolMapConfigEnttity dbcmap)
        {

            if (dbcmap == null || dbcmap.SubItem == null || !dbcmap.SubItem.ContainsKey(propertyName)) return null;
            var config = dbcmap.SubItem[propertyName];
            return FindValueFromDbc(data, config);
        }

        public decimal? FindValueFromDbc(ExchangeProtocolEntity data, ProtocolMapConfigEnttity config)
        {

            // 记录通道信息，方便日志打印
            var channelString = string.Empty;

            // 记录通道值信息，方便日志打印
            var channelValueString = string.Empty;

            // 记录计算公式信息，方便日志打印
            var formula = string.Empty;
            try
            {
                // 未配置
                if (config.Serial == -1)
                {
                    return null;
                }

                //记录通道信息
                channelString = config.Serial.ToString(CultureInfo.InvariantCulture);
                // 记录计算公式信息
                formula = string.IsNullOrEmpty(config.Formula) ? "null" : config.Formula;

                // 计算结果
                var resultValues = ComputeDbcValue(config, data.VariablesData);
                if (!resultValues.Successed)
                {
                    if (!string.IsNullOrEmpty(resultValues.ErrorMessage) && ErrorEvent != null)
                    {
                        ErrorEvent(string.Format("车辆:{0},md5code:{1},DBC配置公式计算错误:{2}",
                                                 data.Vin, data.Md5Code, resultValues.ErrorMessage), data);
                    }
                    return null;
                }

                return resultValues.Data;


            }
            catch (Exception ex)
            {
                if (ExceptionEvent != null)
                    ExceptionEvent(ex, string.Format("车辆:{0},{1} 查询DBC映射关系表异常,相关信息：通道 {2},通道值:{3},计算公式:{4}",
                                                     data.Vin, config.ItemId, channelString, channelValueString, formula), data);

            }
            return null;
        }


        public void FindValueFromDbc(string propertyName, List<string> subPropertyName, ExchangeProtocolEntity data, ProtocolMapConfigEnttity dbcmap, out List<Dictionary<string, decimal?>> valueDic)
        {
            valueDic = null;
            // 记录通道信息，方便日志打印
            var channelString = string.Empty;
            // 记录通道值信息，方便日志打印
            var channelValueString = string.Empty;
            // 记录计算公式信息，方便日志打印
            var formula = string.Empty;
            try
            {
                #region 查找父节点
                // 获取配置
                if (dbcmap == null || dbcmap.SubItem == null || !dbcmap.SubItem.ContainsKey(propertyName))
                {
                    return;
                }
                var config = dbcmap.SubItem[propertyName];
                #endregion

                #region 查找子节点
                if (config.SubItem == null || config.SubItem.Count == 0)
                {

                    return;
                }
                // 创建结果集
                valueDic = new List<Dictionary<string, decimal?>>();
                #region 循环读取子项
                foreach (var item in config.SubItem.Values)
                {
                    var dictoinary = new Dictionary<string, decimal?>();
                    foreach (var subProperty in subPropertyName)
                    {
                        decimal? value = null;
                        // 读取到子项
                        if (item.SubItem.ContainsKey(subProperty) && item.SubItem[subProperty].Serial != -1)
                        {
                            // 计算子项
                            var resultValue = ComputeDbcValue(item.SubItem[subProperty],
                                                                             data.VariablesData);
                            // 子项计算失败
                            if (!resultValue.Successed)
                            {
                                if (!string.IsNullOrEmpty(resultValue.ErrorMessage) && ErrorEvent != null)
                                {
                                    ErrorEvent(string.Format("md5code:{0},协议ID：{2},父项 {1} 子项:{3} 读取失败:{4}",
                                                             data.Md5Code,
                                                             propertyName, 8, subProperty,
                                                             resultValue.ErrorMessage), data);
                                }
                            }
                            else//计算成功
                            {
                                // 子项赋值
                                value = resultValue.Data;
                            }

                        }
                        // 记录子项
                        dictoinary[subProperty] = value;
                    }
                    // 添加到结果集
                    valueDic.Add(dictoinary);
                }
                #endregion 循环读取子项


                #endregion 查找子节点

            }
            catch (Exception ex)
            {
                if (ExceptionEvent != null)
                    ExceptionEvent(ex, string.Format("车辆:{0},{1} 查询DBC映射关系表异常,相关信息：通道 {2},通道值:{3},计算公式:{4}",
                                                     data.Vin, propertyName, channelString, channelValueString, formula), data);

            }
        }

        public void FindValueFromDbc(string propertyName, ExchangeProtocolEntity data, ProtocolMapConfigEnttity dbcmap, out List<decimal?> valueList)
        {
            valueList = null;
            // 记录通道信息，方便日志打印
            var channelString = string.Empty;
            // 记录通道值信息，方便日志打印
            var channelValueString = string.Empty;
            // 记录计算公式信息，方便日志打印
            var formula = string.Empty;
            try
            {
                #region 查找节点
                // 获取配置

                if (dbcmap == null || dbcmap.SubItem == null || !dbcmap.SubItem.ContainsKey(propertyName))
                {
                    return;
                }

                #endregion

                var configData = dbcmap.SubItem[propertyName];


                #region 计算数据结果
                if (configData.Serials == null) return;
                //记录通道信息
                channelString = configData.Serials != null ? string.Join(",", configData.Serials) : "null";
                // 记录计算公式信息
                formula = string.IsNullOrEmpty(configData.Formula) ? "null" : configData.Formula;

                var resultData = GetDbcValues(configData, data.VariablesData);

                if (!resultData.Successed)
                {

                    if (!string.IsNullOrEmpty(resultData.ErrorMessage) && ErrorEvent != null)
                        ErrorEvent(string.Format("车辆:{0},md5code:{1},协议ID：{3},获取多通道数据失败:{2}",
                                                 data.Vin, data.Md5Code,
                                                 resultData.ErrorMessage, 8), data);


                    return;
                }

                valueList = resultData.Data;

                #endregion
            }
            catch (Exception ex)
            {
                if (ExceptionEvent != null)
                    ExceptionEvent(ex, string.Format("车辆:{0},{1} 查询DBC映射关系表异常,相关信息：通道 {2},通道值:{3},计算公式:{4}",
                                                     data.Vin, propertyName, channelString, channelValueString, formula),
                                   data);
            }
        }

        public bool CheckDbcMap(string propertyName, ExchangeProtocolEntity data, ProtocolMapConfigEnttity dbcmap)
        {
            return dbcmap != null && dbcmap.SubItem != null && dbcmap.SubItem.ContainsKey(propertyName);
        }


        public bool CheckDbcMapParent(string propertyName, ExchangeProtocolEntity data, ProtocolMapConfigEnttity dbcmap)
        {
            return dbcmap != null && dbcmap.SubItem != null && dbcmap.SubItem.ContainsKey(propertyName);
        }


        public void FindValueFromDbc(string propertyName, string subPropertyName, ExchangeProtocolEntity data, ProtocolMapConfigEnttity dbcmap, out List<decimal?> valueList)
        {
            valueList = null;
            // 记录通道信息，方便日志打印
            var channelString = string.Empty;
            // 记录通道值信息，方便日志打印
            var channelValueString = string.Empty;
            // 记录计算公式信息，方便日志打印
            var formula = string.Empty;
            try
            {
                #region 查找父节点
                // 获取配置
                if (dbcmap == null || dbcmap.SubItem == null || dbcmap.SubItem.Count == 0)
                {
                    return;
                }


                #endregion

                var parentConfig = dbcmap.SubItem[propertyName];

                #region 查找子节点
                if (parentConfig.SubItem == null ||
                    parentConfig.SubItem.Count == 0)
                {
                    return;
                }
                var configData = parentConfig.SubItem.Values.Where(item => item.SubItem.ContainsKey(subPropertyName)).Select(item => item.SubItem[subPropertyName]).FirstOrDefault();
                #endregion

                #region 计算数据结果
                if (configData == null || configData.Serials == null) return;
                //记录通道信息
                channelString = configData.Serials != null ? string.Join(",", configData.Serials) : "null";
                // 记录计算公式信息
                formula = string.IsNullOrEmpty(configData.Formula) ? "null" : configData.Formula;

                var resultData = GetDbcValues(configData, data.VariablesData);

                if (!resultData.Successed)
                {

                    if (!string.IsNullOrEmpty(resultData.ErrorMessage) && ErrorEvent != null)
                        ErrorEvent(string.Format("车辆:{0},md5code:{1},协议ID：{3},获取多通道数据失败:{2}",
                                                                                 data.Vin, data.Md5Code, resultData.ErrorMessage, 8), data);
                    return;
                }

                valueList = resultData.Data;

                #endregion
            }
            catch (Exception ex)
            {
                if (ExceptionEvent != null)
                    ExceptionEvent(ex, string.Format("车辆:{0},{1} 查询DBC映射关系表异常,相关信息：通道 {2},通道值:{3},计算公式:{4}",
                                                     data.Vin, propertyName, channelString, channelValueString, formula),
                                   data);

            }
        }

        private readonly static Regex FormulaRegex = new Regex(@"(?<=\[)(?<tag>\d*)(?=\])");
        private readonly static DataTable Computer = new DataTable();

        public decimal? Compute(string formula, int[] indexList, decimal?[] data)
        {
            if (indexList.Any(find => find >= data.Length && find < 0))
                return null;
            if (data == null)
                throw new ArgumentException("data");
            if (indexList == null)
                throw new ArgumentException("indexList");
            if (!FormulaRegex.IsMatch(formula))
                throw new ArgumentException("invalid source");
            var newString = new StringBuilder(formula);
            var hasMath = new List<string>();
            // 替换通道值
            foreach (Match match in FormulaRegex.Matches(formula))
            {
                if (hasMath.Contains(match.Value)) continue;

                hasMath.Add(match.Value);
                var index = int.Parse(match.Value);
                if (index >= indexList.Length)
                    throw new ArgumentException(string.Format("length:{1} less than index:{0}", index, indexList.Length), "indexList");

                if (indexList[index] >= data.Length)
                    throw new ArgumentException(string.Format("data length:{1} less than index:{0}", index, data.Length), "indexList");
                newString = newString.Replace("[" + match.Value + "]", data[indexList[index]].ToString());
            }
            try
            {
                return Convert.ToDecimal(Computer.Compute(newString.ToString().Trim(), string.Empty));
            }
            catch (Exception)
            {
                throw new ArgumentException("invalid source" + newString);
            }
        }

        /// <summary>
        /// 计算DBC通道值，如果未填写公式则按照公式计算
        /// </summary>
        /// <param name="config">配置信息</param>
        /// <param name="data">CAN通道</param>
        /// <exception cref="ArgumentNullException">参数异常</exception>
        /// <returns>结果信息</returns>
        public OperationResult<decimal?> ComputeDbcValue(ProtocolMapConfigEnttity config, decimal?[] data)
        {
            var result = new OperationResult<decimal?>();
            if (data == null)
                throw new ArgumentNullException("data");
            if (config == null)
                throw new ArgumentNullException("config");
            if (config.Serials == null)
                return result.False(string.Format("项:{0} 未配置映射关系", config.ItemId));

            if (string.IsNullOrEmpty(config.Formula))
            {
                if (config.Serial > data.Length)
                {
                    throw new ArgumentNullException(string.Format("配置通道为:{0},大于数据通道总数:{1}", config.Serial,
                                                                  data == null ? 0 : data.Length));
                }
                return result.True(data[config.Serial - 1]);
            }
            try
            {
                var value = Compute(config.Formula, config.Serials.Select(index => index - 1).ToArray(), data);
                return result.True(value);
            }
            catch (Exception ex)
            {
                return result.False(ex.ToString());
            }
        }

        /// <summary>
        /// 获一组取DBC通道值,若存在计算公式则必须保证计算公式为一元计算（即只有一个通道参与公式计算），其他值的计算方式与第一个通道相同。
        /// </summary>
        /// <param name="config">配置信息</param>
        /// <param name="data">CAN通道</param>
        /// <returns>结果信息</returns> 
        public OperationResult<List<decimal?>> GetDbcValues(ProtocolMapConfigEnttity config, decimal?[] data)
        {
            var result = new OperationResult<List<decimal?>>();
            if (data == null)
                throw new ArgumentNullException("data");
            if (config == null)
                throw new ArgumentNullException("config");
            if (config.Serials == null)
                return result.False(string.Format("项:{0} 未配置映射关系", config.ItemId));

            // 判断是否需要公式计算
            var values = string.IsNullOrEmpty(config.Formula) ?
                config.Serials.Select(index => index < 1 || index > data.Length ? null : data[index - 1]).ToList() : //直接返回数据
                config.Serials.Select(index => Compute(config.Formula, new[] { index - 1 }, data)).// 返回计算后的数据
                Select(value => value).ToList();// 类型转换
            return result.True(values);
        }
    }
}
