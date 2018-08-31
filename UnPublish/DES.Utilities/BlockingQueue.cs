using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DES.Utilities
{
    /// <summary>
    /// 线程安全队列处理类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class BlockingQueue<TEntity> where TEntity : class
    {
        //缓存队列
        private readonly BlockingCollection<TEntity> _entitiesPool;
        //处理事件
        private readonly Func<List<TEntity>, bool> _dowork;
        //队列容量
        private readonly int _doCapacity;
        //处理状态
        private bool _isBroken = false;
        //文件夹目录
        private readonly string _recoveryDir;
        //Running线程状态
        // ReSharper disable once StaticFieldInGenericType
        public static CancellationTokenSource CancelRunTokenSource = new CancellationTokenSource();

        public BlockingQueue(Func<List<TEntity>, bool> dowork, string recoveryDir = "Recovery", int capacity = 10000, int doCapacity = 1000)
        {
            _entitiesPool = new BlockingCollection<TEntity>(capacity);
            _dowork = dowork;
            _doCapacity = doCapacity;
            _recoveryDir = recoveryDir;
            Task.Factory.StartNew(Dowork);
        }
        /// <summary>
        /// 处理事件主逻辑
        /// </summary>
        private void Dowork()
        {
            while (!CancelRunTokenSource.IsCancellationRequested)
            {
                var list = new List<TEntity>();
                var count = _doCapacity;
                TEntity entity;
                while (count != 0 && _entitiesPool.TryTake(out entity, 300))
                {
                    if (entity == null) continue;
                    list.Add(entity);
                    count--;
                }
                if (list.Count == 0) continue;
                if (_isBroken)
                {
                    //处理状态：失败
                }
                else
                {
                    try
                    {
                        if (!_dowork(list))
                        {
                            _isBroken = true;
                            //Todo：开始记录本地文件...
                            foreach (var brokenEntity in list)
                            {
                                Push(brokenEntity);
                            }
                        }
                        else
                        {
                            //处理状态：成功
                        }
                    }
                    catch (Exception ex)
                    {
                        _isBroken = true;
                        //Todo： 记录异常

                    }
                }
                Thread.Sleep(100);
            }
        }

        /// <summary>
        /// 存入缓存队列
        /// </summary>
        /// <param name="entity"></param>
        public void Push(TEntity entity)
        {
            _entitiesPool.Add(entity);
        }
    }
}
