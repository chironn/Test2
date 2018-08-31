/* ==============================================================================
* 类型名称：MsgKeyDefinition  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/28 16:05:57
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

namespace DES.Utilities.KeyDefinition
{
    /// <summary>
    /// 通信队列中的键值对的键名定义
    /// </summary>
    public sealed partial class MsgKeyDefinition
    {
        /// <summary>
        /// 转发接入用户名
        /// </summary>
        public const string ACCESSINFOID = "ACCESSINFOID";

        /// <summary>
        /// 转发服务输出类型，1：tcp通信输出，2：国标国家平台文件数据，3：国标深圳平台文件数据，4：宁德数据转换形式。
        /// </summary>
        public const string OUTPUTTYPE = "OUTPUTTYPE";

        /// <summary>
        /// 经度
        /// </summary>
        public const string LONGITUDE = "LONGITUDE";

        /// <summary>
        /// 纬度
        /// </summary>
        public const string LATITUDE = "LATITUDE";

        /// <summary>
        /// dbc文件的MD5值
        /// </summary>
        public const string MD5CODE = "MD5CODE";

        /// <summary>
        /// 海拔
        /// </summary>
        public const string ALTITUDE = "ALTITUDE";

        /// <summary>
        /// GPS方向
        /// </summary>
        public const string DIRECTION = "DIRECTION";


        /// <summary>
        /// DBC映射关系，相关信息
        /// </summary>
        public const string DBCMAP = "DBCMAP";



    }
}
