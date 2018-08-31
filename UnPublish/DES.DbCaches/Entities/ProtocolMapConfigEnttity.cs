/* ==============================================================================
* 类型名称：ProtocolMapConfigEnttity  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/30 15:28:55
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace DES.DbCaches.Entities
{
    /// <summary>
    /// 协议DBC映射关系项
    /// </summary>
    public class ProtocolMapConfigEnttity
    {
        private static ProtocolMapConfigEnttity ParseJObject(JObject obj)
        {
            var entity = new ProtocolMapConfigEnttity
            {
                ItemId = obj["text"].ToString(),
                SerialString = obj["CheckValue"].ToString(),
                Formula = obj["Formula"] != null ? obj["Formula"].ToString() : string.Empty,
                ConfigType = obj["AlarmType"] != null ? obj["AlarmType"].ToString() : "0",
            };
            if (obj["children"] != null && obj["children"].HasValues)
            {
                var array = JArray.Parse(obj["children"].ToString());
                entity.SubItem = new Dictionary<string, ProtocolMapConfigEnttity>();
                foreach (var subEntity in array.Select(item => ParseJObject((JObject)item)).Where(subEntity => !string.IsNullOrEmpty(subEntity.ItemId)))
                    entity.SubItem[subEntity.ItemId] = subEntity;
            }

            return entity;
        }


        public static ProtocolMapConfigEnttity Parse(string line)
        {
            var objList = JToken.Parse(line);

            var entity = new ProtocolMapConfigEnttity
            {
                ItemId = string.Empty,
                SerialString = string.Empty,
                Formula = string.Empty,
                SubItem = new Dictionary<string, ProtocolMapConfigEnttity>(),
            };

            foreach (var child in from JObject obj in objList select ParseJObject(obj))
                entity.SubItem[child.ItemId] = child;
            return entity;
        }

        public static bool TryParse(string line, out ProtocolMapConfigEnttity entity)
        {
            try
            {
                entity = Parse(line);
                entity.HashCode = line.GetHashCode();
                return true;
            }
            catch
            {
                entity = null;
            }
            return false;
        }


        private string _serialString;
        private int[] _serials;
        private int _serial;
        /// <summary>
        /// 协议项名称
        /// </summary>
        public string ItemId { get; set; }
        /// <summary>
        /// DBC通道编号从1开始，若有多个用逗号隔开
        /// </summary>
        public string SerialString
        {
            get { return _serialString; }
            set
            {
                _serialString = value;
                _serial = -1;
                if (string.IsNullOrEmpty(_serialString) || int.TryParse(_serialString, out _serial))
                {
                    _serials = new[] { _serial };

                }
                else
                {
                    var splits = _serialString.Split(',');
                    if (_serials == null)
                        _serials = new int[splits.Length];
                    var index = 0;
                    foreach (var split in splits.Where(split => !string.IsNullOrEmpty(split)))
                    {
                        int serial;
                        if (int.TryParse(split, out serial))
                            _serials[index++] = serial;
                    }
                    if (index == 0)
                        _serials = new[] { -1 };
                    else if (index != Serials.Length)
                        Array.Resize(ref _serials, index);
                    _serial = _serials[0];
                }
            }
        }

        /// <summary>
        /// DBC通道编号从1开始
        /// </summary>
        public int Serial { get { return _serial; } }

        /// <summary>
        /// DBC通道编号列表
        /// </summary>
        public int[] Serials { get { return _serials; } }

        public int HashCode { get; set; }

        public string Formula { get; set; }

        /// <summary>
        /// 改配置类型。0通道信息，1故障码信息，2禁用
        /// <remarks>序列化名称：AlarmType</remarks>
        /// </summary>
        public string ConfigType { set; get; }

        /// <summary>
        /// 子项
        /// </summary>
        public Dictionary<string, ProtocolMapConfigEnttity> SubItem { get; set; }

    }
}
