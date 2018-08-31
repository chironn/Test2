/* ==============================================================================
 * 类型名称：InvokeWacherManager  
 * 类型描述：方法调用监控器管理类
 * 创 建 者：linfk
 * 创建日期：2018/1/26 18:58:38
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/

using System.Collections.Generic;
using System.Linq;

namespace DES.Core.Diagnostics
{
    /// <summary>
    /// 方法调用监控器管理类
    /// </summary>
    public class InvokeWatcherManager
    {
        protected System.Collections.Concurrent.ConcurrentDictionary<string, InvokeWatcher> InvokeWatcherDic =
            new System.Collections.Concurrent.ConcurrentDictionary<string, InvokeWatcher>();

        /// <summary>
        /// 是否开启监控
        /// </summary>
        public bool IsOpen { get; set; }
        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="name">监控器名称</param>
        /// <returns>监控器名称</returns>
        public InvokeWatcher this[string name]
        {
            get
            {
                if (!InvokeWatcherDic.ContainsKey(name))
                {
                    InvokeWatcherDic[name] = new InvokeWatcher();
                }
                InvokeWatcherDic[name].IsOpen = IsOpen;
                return InvokeWatcherDic[name];
            }
        }

        /// <summary>
        /// 返回全部所引器
        /// </summary>
        /// <returns></returns>
        public IEnumerable<InvokeWatcher> All()
        {
            return InvokeWatcherDic.Values.AsEnumerable();
        }

        /// <summary>
        /// 当前索引器总数
        /// </summary>
        public int Count
        {
            get { return InvokeWatcherDic.Count; }
        }
    }
}
