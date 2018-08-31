/* ==============================================================================
* 类型名称：MqConfig  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/19 15:15:38
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using DES.Core.Interfaces;

namespace DES.CommunicationMq.Entities
{
    /// <summary>
    /// 默认的MQ配置信息实体
    /// </summary>
    public class MqConfig : IQueueConfig
    {
        public string FindKey { get; set; }
        public string RoleType { get; set; }

        public string QueueName { get; set; }

        public string BindKey { get; set; }

        public string RouteingKey { get; set; }

        public string ExchangeName { get; set; }

        public string ExchangeType { get; set; }

        public string ServerIp { get; set; }

        public ushort Port { get; set; }

        public string UserId { get; set; }

        public string Password { get; set; }

        public bool ExchangeDurable { get; set; }




        /// <summary>
        /// 克隆浅副本
        /// </summary>
        /// <returns>副本</returns>
        public MqConfig Clone()
        {
            return (MqConfig)MemberwiseClone();
        }

        public override string ToString()
        {
            var result = new System.Text.StringBuilder();
            result.AppendFormat("角色:{0},服务地址:{1},交换名:{2},绑定/路由键{3} ",
                RoleType, string.Format("{0}:{1}", ServerIp, Port), ExchangeName, string.IsNullOrEmpty(RouteingKey) ? RouteingKey : BindKey);

            if (System.String.CompareOrdinal("producer", RoleType) != 0)
            {
                result.AppendFormat("交换类型:{0} 队列名称:{1}", ExchangeType, QueueName);
            }

            return result.ToString();

        }



        public string GetRouteing()
        {
            return string.Format("{0},{1}", ExchangeName, string.IsNullOrEmpty(BindKey) ? RouteingKey : BindKey);
        }
    }
}
