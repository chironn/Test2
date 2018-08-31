/* ==============================================================================
* 类型名称：MsgKeyDefinition_2  
* 类型描述：键值对定义
* 创 建 者：linfk
* 创建日期：2017/12/19 9:37:09
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

namespace DES.Utilities.KeyDefinition
{
    /// <summary>
    /// 实时数据转发相关键值定义
    /// </summary>
    public partial class MsgKeyDefinition
    {
        /// <summary>
        /// 通信实体内部实体消息ID
        /// </summary>
        public const string SUBPR = "SUBPR";
        /// <summary>
        /// 应答数据ID
        /// </summary>
        public const string ANSWERID = "ANSWERID";
        /// <summary>
        /// 转发服务SEQ
        /// </summary>
        public const string FORWARDSEQ = "FORWARDSEQ";
    }
}
