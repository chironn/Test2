/* ==============================================================================
* 类型名称：CmdActionAttribute  
* 类型描述：命令行指令标签
* 创 建 者：linfk
* 创建日期：2017/12/8 9:41:26
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;

namespace DES.Core
{
    /// <summary>
    /// 命令行指令标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CmdActionAttribute : Attribute
    {
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; private set; }

        /// <summary>
        /// 指令名称
        /// </summary>
        public string Cmd { get; private set; }

        /// <summary>
        /// 操作方法
        /// </summary>
        public DoCmd DoCmdCallback { get; set; }

        /// <summary>
        /// 构造
        /// </summary>
        /// <param name="cmd">指令名称</param>
        /// <param name="remark">备注</param>
        public CmdActionAttribute(string cmd, string remark)
        {
            Cmd = cmd;
            Remark = remark;
        }
    }

    /// <summary>
    /// 控制命令执行委托
    /// </summary>
    /// <param name="param">参数字符串</param>
    /// <param name="shell">当前运行的引用程序外壳</param>
    /// <param name="servcie">当前运行的服务主体</param>
    public delegate void DoCmd(string param, Shell shell, BaseService servcie);
}
