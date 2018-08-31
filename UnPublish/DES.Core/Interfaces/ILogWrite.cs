/* ==============================================================================
 * 类型名称：ILogWrite  
 * 类型描述：日志读写相关操作
 * 创 建 者：linfk
 * 创建日期：2017/11/17 17:20:08
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/
using System;

namespace DES.Core.Interfaces
{
    /// <summary>
    /// 日志读写相关操作
    /// </summary>
    public interface ILogWrite
    {
        /// <summary>
        /// 日志当前环境，会在单行日志的首部进行显示
        /// </summary>
        string Context { get; }
        /// <summary>
        /// 记录一条提示信息
        /// </summary>
        /// <param name="message">消息</param>
        void WriteInfo(string message);
        /// <summary>
        /// 记录一条警告信息
        /// </summary>
        /// <param name="message">消息</param>
        void WriteWarn(string message);
        /// <summary>
        /// 记录一条错误信息
        /// </summary>
        /// <param name="message">消息</param>
        void WriteError(string message);
        /// <summary>
        /// 记录一条带异常的错误信息
        /// </summary>
        /// <param name="ex">异常</param>
        /// <param name="message">附加信息</param>
        void WriteError(Exception ex, string message = "");

        /// <summary>
        /// 记录一条带错误码的错误信息
        /// </summary>
        /// <param name="errorCode">错误码</param>
        /// <param name="append">附加信息</param>
        /// <param name="param">其他操作</param>
        void Write(string errorCode, string append,params object[] param);

    }
}
