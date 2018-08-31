
/* ==============================================================================
* 类型名称：InLogWrite  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/27 9:30:34
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using DES.Core.Interfaces;

namespace DES.InLog
{
    /// <summary>
    /// 
    /// </summary>
    public class InLogWrite : ILogWrite
    {
        public string Context
        {
            get;
            private set;
        }

        public InLogWrite(string context)
        {
            Context = context;
        }

        public void WriteInfo(string message)
        {
            inCom.Logs.Log4NetHelper.Info(string.Format("{0};{1}", Context, message));
        }

        public void WriteWarn(string message)
        {
            inCom.Logs.Log4NetHelper.Warn(string.Format("{0};{1}", Context, message));
        }

        public void WriteError(string message)
        {
            inCom.Logs.Log4NetHelper.Error(string.Format("{0};{1}", Context, message));
        }

        public void WriteError(Exception ex, string message = "")
        {
            inCom.Logs.Log4NetHelper.Error(string.Format("{0};{1};异常:{2}", Context, message, ex == null ? string.Empty : ex.ToString()));
        }

        public void Write(string errorCode, string append, params object[] param)
        {
            inCom.Logs.Log4NetHelper.Error(errorCode, append, param);
        }
    }
}
