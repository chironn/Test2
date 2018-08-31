/* ==============================================================================
* 类型名称：FastConsumer  
* 类型描述：快速消费者
* 创 建 者：linfk
* 创建日期：2018/1/12 10:29:50
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DES.CommunicationMq.MqQueue
{
    /// <summary>
    /// 快速消费者
    /// </summary>
    public class FastConsumer : DefaultBasicConsumer
    {
        /// <summary>
        /// 消息抵达事件
        /// </summary>
        public event EventHandler<BasicDeliverEventArgs> BasicDeliverEvnet;

        /// <summary>
        /// 创建快速消费实体
        /// </summary>
        public FastConsumer(IModel model)
            : base(model)
        {
        }

        /// <summary>
        /// Overrides <see cref="DefaultBasicConsumer"/>'s  <see cref="HandleBasicDeliver"/> implementation,
        ///  building a <see cref="BasicDeliverEventArgs"/> instance and placing it in the Queue.
        /// </summary>
        public override void HandleBasicDeliver(string consumerTag,
            ulong deliveryTag,
            bool redelivered,
            string exchange,
            string routingKey,
            IBasicProperties properties,
            byte[] body)
        {
            var eventArgs = new BasicDeliverEventArgs
            {
                ConsumerTag = consumerTag,
                DeliveryTag = deliveryTag,
                Redelivered = redelivered,
                Exchange = exchange,
                RoutingKey = routingKey,
                BasicProperties = properties,
                Body = body
            };

            if (BasicDeliverEvnet != null)
                BasicDeliverEvnet(this, eventArgs);
        }

    }
}
