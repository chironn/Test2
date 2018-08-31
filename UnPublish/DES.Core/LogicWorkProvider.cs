/* ==============================================================================
* 类型名称：LogicWorkProvider  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/11/17 9:19:54
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/


using System.Linq;
using System.Collections.Generic;

using DES.Core.Interfaces;


namespace DES.Core
{
    /// <summary>
    /// 逻辑工程提供类
    /// </summary>
    public class LogicWorkProvider
    {
        /// <summary>
        /// 逻辑组件查找字典
        /// </summary>
        public System.Collections.Concurrent.ConcurrentDictionary<string, ILogicWork> LogicDic { get; protected set; }

        /// <summary>
        /// 消息订阅列表
        /// </summary>
        public System.Collections.Concurrent.ConcurrentDictionary<string, List<ILogicWork>> SubscribeDic { get; protected set; }

        public System.Collections.Concurrent.ConcurrentDictionary<string, IToLogic> ParseToLogic { get; set; }
        public System.Collections.Concurrent.ConcurrentDictionary<string, IToCommunication> ParseToCommunication { get; set; }

        /// <summary>
        /// 默认的逻辑转换实体
        /// </summary>
        public IToLogic DefaultParseLogic;
        //public IToCommunication DefaultParseToCommunication =？

        /// <summary>
        /// 根据程序级加载逻辑处理组件
        /// </summary>
        /// <param name="service">当前服务运行服务</param>
        /// <param name="assembly">程序集</param>
        /// <returns>逻辑处理组件</returns>
        public virtual IEnumerable<ILogicWork> LoadAssembly(BaseService service, System.Reflection.Assembly assembly)
        {
            if (LogicDic == null)
            {
                LogicDic = new System.Collections.Concurrent.ConcurrentDictionary<string, ILogicWork>();
            }

            if (SubscribeDic == null)
            {
                SubscribeDic = new System.Collections.Concurrent.ConcurrentDictionary<string, List<ILogicWork>>();
            }

            if (ParseToLogic == null)
                ParseToLogic = new System.Collections.Concurrent.ConcurrentDictionary<string, IToLogic>();

            if (ParseToCommunication == null)
                ParseToCommunication = new System.Collections.Concurrent.ConcurrentDictionary<string, IToCommunication>();

            foreach (var logic in Register.ReflectInstance<ILogicWork>(assembly))
            {
                logic.Service = service;
                LogicDic[logic.FindKey] = logic;
                if (logic.SubscribeList == null) continue;
                foreach (var findkey in logic.SubscribeList)
                {
                    if (!SubscribeDic.ContainsKey(findkey))
                        SubscribeDic[findkey] = new List<ILogicWork>();
                    SubscribeDic[findkey].Add(logic);
                }
            }

            foreach (var parse in Register.ReflectInstance<IToLogic>(assembly))
            {
                parse.Service = service;
                ParseToLogic[parse.FindKey] = parse;
            }

            DefaultParseLogic = new MsgToLogic { Service = service };

            foreach (var parse in Register.ReflectInstance<IToCommunication>(assembly))
            {
                parse.Service = service;
                ParseToCommunication[parse.FindKey] = parse;
            }

            return LogicDic.Values.ToList();
        }
    }
}
