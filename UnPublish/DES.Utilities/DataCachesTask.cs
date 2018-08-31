using System;
using System.Threading.Tasks;

namespace DES.Utilities
{
    /// <summary>
    /// 数据队列，异步缓存处理对象，每个实体对象启动会产生一个队列和一个线程，处理缓存中的数据
    /// </summary>
    public class DataCachesTask<TEntity> : IDisposable
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 队列长度
        /// </summary>
        public int QueueCount { get { return _entityQueue.Count; } }

        /// <summary>
        /// 数据队列
        /// </summary>
        private System.Collections.Concurrent.BlockingCollection<TEntity> _entityQueue =
            new System.Collections.Concurrent.BlockingCollection<TEntity>();
        /// <summary>
        /// 数据操作回调
        /// </summary>
        private readonly Action<TEntity> _doworkCallback;
        /// <summary>
        /// 数据异常回调
        /// </summary>
        private readonly Action<Exception> _exceptionCallback;
        private System.Threading.CancellationTokenSource _mainToken = new System.Threading.CancellationTokenSource();

        /// <summary>
        /// 捕获处理线程异常
        /// </summary>
        /// <param name="entity">数据实体</param>
        private void TryCatchDowork(TEntity entity)
        {
            try
            {
                _doworkCallback(entity);
            }
            catch (Exception ex)
            {
                if (_exceptionCallback != null)
                    _exceptionCallback(new Exception(string.Format("{0},type:{1}，处理操作[TryCatchDowork]异常:{2}",
                                                                   string.IsNullOrEmpty(Name) ? "未命名" : Name,
                                                                   GetType().Name, ex)));
            }
        }

        // 循环执行操作
        private void CycleWorking()
        {
            var curentThread = System.Threading.Thread.CurrentThread;
            curentThread.Priority = System.Threading.ThreadPriority.Highest;
            while (_mainToken != null && !_mainToken.IsCancellationRequested)
            {
                TEntity entity;
                // 使用阻塞方法，保证线程不会长时间占用CPU
                if (_entityQueue.TryTake(out entity, 3000))
                {
                    TryCatchDowork(entity);
                }
            }
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="doworkCallback">数据操作回调</param>
        /// <param name="exceptionCallback">异常处理回调</param>
        public DataCachesTask(Action<TEntity> doworkCallback, Action<Exception> exceptionCallback)
        {
            Name = "未命名";
            _doworkCallback = doworkCallback;
            _exceptionCallback = exceptionCallback;
            Task.Factory.StartNew(CycleWorking, _mainToken.Token);


        }

        /// <summary>
        /// 添加实体到到异步处理队列
        /// </summary>
        /// <param name="entity">实体对象</param>
        public void Push(TEntity entity)
        {
            _entityQueue.Add(entity);
        }

        #region IDisposable
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            if (_entityQueue != null)
                _entityQueue.Dispose();
            _entityQueue = null;

            if (_mainToken == null) return;
            _mainToken.Cancel();
            _mainToken.Dispose();
            _mainToken = null;
        }
        #endregion
    }
}