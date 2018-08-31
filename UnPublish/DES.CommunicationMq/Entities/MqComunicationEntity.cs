/* ==============================================================================
* 类型名称：MqComunicationEntity  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/28 10:11:30
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using DES.Core;

namespace DES.CommunicationMq.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class MqComunicationEntity : BufferEntity
    {
        /// <summary>
        /// 交换名称
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// 路由键
        /// </summary>
        public string RouteingKey { get; set; }

        /// <summary>
        /// 设置路由信息，由‘,’分割，第一个为 ExchangeName,第二个为 RouteingKey
        /// </summary>
        /// <param name="routeing"></param>
        public override void SetRouteing(string routeing)
        {
            var splits = routeing.Split(',');
            ExchangeName = splits[0];
            RouteingKey = splits[1];
        }
        /// <summary>
        /// 返回路由键（路由信息，由‘,’分割，第一个为 ExchangeName,第二个为 RouteingKey）
        /// </summary>
        /// <returns>路由信息</returns>
        public override string GetRoteing()
        {
            return string.Format("{0},{1}", ExchangeName, RouteingKey);
        }
    }
}
