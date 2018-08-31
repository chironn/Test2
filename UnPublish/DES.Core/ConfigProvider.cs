/* ==============================================================================
* 类型名称：ConfigProvider  
* 类型描述：配置文件工厂类
* 创 建 者：linfk
* 创建日期：2017/11/16 14:16:00
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System;
using System.IO;

namespace DES.Core
{
    /// <summary>
    /// 配置文件工厂类
    /// </summary>
    public class ConfigProvider
    {
        public virtual string LoadConfig(string configName)
        {
            var filePath = string.Format(@"{0}IniFiles\{1}",
                                         AppDomain.CurrentDomain.BaseDirectory, configName);
            return File.ReadAllText(filePath);
        }
    }
}
