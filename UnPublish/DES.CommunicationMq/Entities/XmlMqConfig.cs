/* ==============================================================================
* 类型名称：XmlMqConfig  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/19 15:18:19
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DES.Core.Interfaces;

namespace DES.CommunicationMq.Entities
{

    [XmlRoot("XmlMqConfigArray", IsNullable = false)]
    public class XmlMqConfigArray
    {
        public XmlMqConfigArray() { }

        public XmlMqConfigArray(params XmlMqConfig[] config)
        {
            XmlMqConfigs = config;
        }

        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <param name="line">序列化字符</param>
        /// <param name="config">配置信息</param>
        /// <returns>序列化结果</returns>
        public static bool TryParse(string line, out XmlMqConfigArray config)
        {
            config = null;
            try
            {

                var tx = new StringReader(line);
                var xmlSerializer = new XmlSerializer(typeof(XmlMqConfigArray));
                config = (XmlMqConfigArray)xmlSerializer.Deserialize(tx);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ToString(XmlMqConfigArray config)
        {
            try
            {

                var tx = new StringWriter();
                var xmlSerializer = new XmlSerializer(typeof(XmlMqConfigArray));
                xmlSerializer.Serialize(tx, config);
                return tx.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }


        [XmlElementAttribute("XmlMqConfig", IsNullable = false)]
        public XmlMqConfig[] XmlMqConfigs { get; set; }
    }

    /// <summary>
    ///  MQ队列 xml 配置信息
    /// </summary>
    [XmlRoot("XmlMqConfig", IsNullable = false)]
    public class XmlMqConfig
    {

        /// <summary>
        /// 解析配置文件
        /// </summary>
        /// <param name="line">序列化字符</param>
        /// <param name="config">配置信息</param>
        /// <returns>序列化结果</returns>
        public static bool TryParse(string line, out XmlMqConfig config)
        {
            config = null;
            try
            {

                var tx = new StringReader(line);
                var xmlSerializer = new XmlSerializer(typeof(XmlMqConfig));
                config = (XmlMqConfig)xmlSerializer.Deserialize(tx);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 服务地址
        /// </summary>
        [XmlAttribute("ServerIp")]
        public string ServerIp { get; set; }

        /// <summary>
        /// 服务端口
        /// </summary>
        [XmlAttribute("Port")]
        public ushort Port { get; set; }

        /// <summary>
        /// 服务用户名
        /// </summary>
        [XmlAttribute("UserId")]
        public string UserId { get; set; }

        /// <summary>
        /// 服务密码
        /// </summary>
        [XmlAttribute("Password")]
        public string Password { get; set; }

        /// <summary>
        /// 消费配置
        /// </summary>
        [XmlElementAttribute("Consumer", IsNullable = false)]
        public Consumer[] Consumers { get; set; }

        /// <summary>
        /// 生产者配置
        /// </summary>
        [XmlElementAttribute("Producer", IsNullable = false)]
        public Producer[] Producers { get; set; }

        public List<IQueueConfig> ToList()
        {
            var result = new List<IQueueConfig>();
            if (Consumers != null)
            {
                result.AddRange(Consumers.Select(consumer => new MqConfig
                {
                    ServerIp = ServerIp,
                    Port = Port,
                    Password = Password,
                    UserId = UserId,
                    FindKey = consumer.FindKey,
                    QueueName = consumer.QueueName,
                    ExchangeName = consumer.ExchangeName,
                    ExchangeType = consumer.ExchangeType,
                    ExchangeDurable=consumer.ExchangeDurable,
                    BindKey = consumer.BindKey,
                    RouteingKey = consumer.BindKey,
                    RoleType = "consumer"
                }));
            }

            if (Producers != null)
            {
                result.AddRange(Producers.Select(producer => new MqConfig
                {
                    ServerIp = ServerIp,
                    Port = Port,
                    Password = Password,
                    UserId = UserId,
                    FindKey = producer.FindKey,
                    ExchangeName = producer.ExchangeName,
                    RouteingKey = producer.RouteingKey,
                    // 绑定键与路由相同
                    BindKey = producer.RouteingKey,
                    RoleType = "producer"
                }));
            }


            return result;

        }
    }

    [XmlRoot("Consumer",  IsNullable = false)]
    public class Consumer
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Consumer()
        {
            ExchangeDurable = true;
        }

        /// <summary>
        /// 查找名称
        /// </summary>
        [XmlAttribute("FindKey")]
        public string FindKey { get; set; }
        /// <summary>
        /// 队列名
        /// </summary>
        [XmlAttribute("QueueName")]
        public string QueueName { get; set; }
        /// <summary>
        /// 绑定键
        /// </summary>
        [XmlAttribute("BindKey")]
        public string BindKey { get; set; }
        /// <summary>
        /// 交换名
        /// </summary>
        [XmlAttribute("ExchangeName")]
        public string ExchangeName { get; set; }
        /// <summary>
        /// 交换类型
        /// </summary>
        [XmlAttribute("ExchangeType")]
        public string ExchangeType { get; set; }

        /// <summary>
        /// 交换是否持久化
        /// </summary>
        [XmlAttribute("ExchangeDurable")]
        public bool ExchangeDurable { get; set; }
    }

    [XmlRoot("Producer",  IsNullable = false)]
    public class Producer
    {
        /// <summary>
        /// 查找名称
        /// </summary>
        [XmlAttribute("FindKey")]
        public string FindKey { get; set; }

        /// <summary>
        /// 交换名
        /// </summary>
        [XmlAttribute("ExchangeName")]
        public string ExchangeName { get; set; }

        /// <summary>
        /// 路由键
        /// </summary>
        [XmlAttribute("RouteingKey")]
        public string RouteingKey { get; set; }
    }
}
