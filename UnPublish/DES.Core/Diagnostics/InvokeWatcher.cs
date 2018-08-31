/* ==============================================================================
 * 类型名称：InvokeWatcher  
 * 类型描述：方法调用监控器
 * 创 建 者：linfk
 * 创建日期：2018/1/26 18:31:35
 * =====================
 * 修改者：
 * 修改描述：
 # 修改日期
 * ==============================================================================*/
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace DES.Core.Diagnostics
{
    /// <summary>
    /// 方法调用监控器
    /// </summary>
    public class InvokeWatcher
    {
        /// <summary>
        /// 是否开启
        /// </summary>
        public bool IsOpen { get; set; }
        private int _samplingCount = 10;
        /// <summary>
        /// 采用总数
        /// </summary>
        public virtual int SamplingCount { get { return _samplingCount; } set { _samplingCount = value; } }
        /// <summary>
        /// 及时器操作
        /// </summary>
        protected readonly Stopwatch _watcher = new Stopwatch();
        /// <summary>
        /// 耗时列表
        /// </summary>
        protected List<double> Elapseds = new List<double>();
        /// <summary>
        /// 最长耗时
        /// </summary>
        public virtual double Max
        {
            get { return Elapseds.Max(); }
        }
        /// <summary>
        /// 最短耗时
        /// </summary>
        public virtual double Min
        {
            get { return Elapseds.Min(); }
        }
        /// <summary>
        /// 平均值
        /// </summary>
        public virtual double Average
        {
            get { return Elapseds.Average(); }
        }
        /// <summary>
        /// 监控调用
        /// </summary>
        /// <param name="delegate">被调用的方法</param>
        protected virtual void WatchInvoke(Action @delegate)
        {
            _watcher.Restart();
            @delegate();
            _watcher.Stop();
            if (Elapseds.Count >= SamplingCount)
            {
                Elapseds.RemoveAt(Elapseds.Count - 1);
            }
            Elapseds.Add(_watcher.Elapsed.TotalMilliseconds);
        }
        /// <summary>
        /// 启动计时器
        /// </summary>
        /// <param name="delegate">需要计时的方法</param>
        public void Invoke(Action @delegate)
        {
            if (IsOpen)
            {
                WatchInvoke(@delegate);
            }
            else
            {
                @delegate();
            }
        }
    }
}
