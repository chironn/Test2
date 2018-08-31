/* ==============================================================================
* 类型名称：LogProvider  
* 类型描述：日志操作工厂类
* 创 建 者：linfk
* 创建日期：2017/11/17 17:27:32
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using DES.Core.Interfaces;

namespace DES.Core
{
    /// <summary>
    /// 日志操作工厂类
    /// </summary>
    public abstract class LogProvider
    {
        /// <summary>
        /// 创建一个日志实例（配置文件位置采用默认路径）
        /// </summary>
        /// <param name="context">实例所在的上下文描述</param>
        /// <returns>日志实例</returns>
        public abstract ILogWrite GetLog(string context);

        /// <summary>
        /// 创建一个日志实例（配置文件位置service实例中获取）
        /// </summary>
        /// <param name="context">实例所在的上下文描述</param>
        /// <param name="service"> 当前服务程序</param>
        /// <returns>日志实例</returns>
        public abstract ILogWrite GetLog(string context, BaseService service);

        public abstract ILogWrite GetLog(string context,string configFilePath);
    }
}
