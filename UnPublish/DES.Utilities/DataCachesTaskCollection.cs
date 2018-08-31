using System;
using System.Collections.Generic;

namespace DES.Utilities
{
    /// <summary>
    /// 异步缓存处理对象集合，用于创建异步线程队列和分发数据到各个队列
    /// </summary>
    public class DataCachesTaskCollection<TEntity> : IDisposable
    {
        /// <summary>
        /// 键值升序索引
        /// </summary>
        private readonly Dictionary<string, long> _keyNumberDic = new Dictionary<string, long>();

        /// <summary>
        /// 更具键值读取或生成键值索引
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>生成的索引</returns>
        private long GetOrCreateKeyNumber(string key)
        {
            lock (_keyNumberDic)
            {
                if (!_keyNumberDic.ContainsKey(key))
                {
                    // 使用索引作为序列值
                    var number = _keyNumberDic.Count;
                    _keyNumberDic[key] = number;
                }
            }

            return _keyNumberDic[key];
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 返回只读的线程列表
        /// </summary>
        public IList<DataCachesTask<TEntity>> TaskList { get { return _taskList.AsReadOnly(); } }

        // 线程列表
        private readonly List<DataCachesTask<TEntity>> _taskList = new List<DataCachesTask<TEntity>>();

        /// <summary>
        /// 更具输入实体类型，创建实体分发编号，程序使用此编号进行模运算，使用运算结果作为分发入列依据。
        /// </summary>
        private readonly Func<TEntity, string> _createEntityNumberCallback;

        /// <summary>
        /// 数据操作回调
        /// </summary>
        private readonly Action<TEntity> _doworkCallback;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="createEntityNumberCallback">更具输入实体类型，创建实体分发编号，程序使用此编号进行模运算，使用运算结果作为分发入列依据。</param>
        /// <param name="doworkCallback">数据操作回调</param>
        /// <param name="exceptionCallback">异常处理回调 </param>
        /// <param name="ability">线程数量</param>
        public DataCachesTaskCollection(Func<TEntity, string> createEntityNumberCallback, Action<TEntity> doworkCallback, Action<Exception> exceptionCallback, int ability = 2)
        {
            _createEntityNumberCallback = createEntityNumberCallback;
            _doworkCallback = doworkCallback;
            // 创建线程处理列表
            while (ability != 0)
            {
                _taskList.Add(new DataCachesTask<TEntity>(_doworkCallback, exceptionCallback) { Name = Name });
                ability--;
            }
        }

        /// <summary>
        /// 推送数据到队列中
        /// </summary>
        /// <param name="entity"></param>
        public void Push(TEntity entity)
        {
            // 使用列表索引取模，数据会均匀的分布到各列中。
            var number = GetOrCreateKeyNumber(_createEntityNumberCallback(entity));
            var index = (int)(number % _taskList.Count);
            _taskList[index].Push(entity);

        }

        /// <summary>
        /// 是否线程队列
        /// </summary>
        public void Dispose()
        {
            if (_taskList.Count == 0) return;
            foreach (var task in _taskList)
                task.Dispose();
            _taskList.Clear();
        }
    }
}