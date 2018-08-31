using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace DES.Utilities.IO
{
    /// <summary>
    /// 使用异步队列处理文件写入操作，避免影响上层业务逻辑
    /// </summary>
    public class FileWriteManger : IDisposable
    {
        
        

        /// <summary>
        /// 数据异常回调
        /// </summary>
        private readonly Action<Exception> _exceptionCallback;

        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, FileWriter> _fileWriteDic =
            new System.Collections.Concurrent.ConcurrentDictionary<string, FileWriter>();

        /// <summary>
        /// 文件写入操作列表
        /// </summary>
        public System.Collections.Concurrent.ConcurrentDictionary<string, FileWriter> FileWriteDic
        {
            get { return _fileWriteDic; }
        }

        private CancellationTokenSource _mainToken = new CancellationTokenSource();
        private ManualResetEvent _threadWait = new ManualResetEvent(false);



        /// <summary>
        /// 循环刷新文件到本地存储
        /// </summary>
        private void CycleFlush()
        {

            while (_mainToken != null && !_mainToken.Token.IsCancellationRequested)
            {

                ParallelFlush();

                //1 秒刷新一次
                //Thread.Sleep(10 * 1000);
                _threadWait.WaitOne(1 * 1000);
            }
        }
        /// <summary>
        /// 异步写入数据到文本
        /// </summary>
        public void Flush()
        {
            var keys = _fileWriteDic.Keys.ToArray();
            foreach (var path in keys)
            {
                FileSave(path);
            }
        }

        public void ParallelFlush()
        {
            Parallel.ForEach(_fileWriteDic.Keys, FileSave);
        }

        /// <summary>
        /// 指定文件写入刷入缓存。
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void FileSave(string filePath)
        {
            try
            {

                if (!_fileWriteDic.ContainsKey(filePath)) return;
                // 刷新日志到内存
                _fileWriteDic[filePath].Flush();
                // 超过10分钟未活动，关闭文件句柄
                if (_fileWriteDic[filePath].LastActivityTime.AddMinutes(10) >= DateTime.Now) return;
                FileWriter write;
                if (_fileWriteDic.TryRemove(filePath, out write))
                {
                    write.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (_exceptionCallback != null)
                    _exceptionCallback(ex);
            }
        }

        /// <summary>
        /// 向队列推入日志数据
        /// </summary>
        /// <param name="path">日志文件路径</param>
        /// <param name="line">日志文件内容</param>
        public void Push(string path, string line)
        {
            if (!_fileWriteDic.ContainsKey(path))
            {
                lock (_fileWriteDic)
                    _fileWriteDic[path] = new FileWriter(path);
            }
            _fileWriteDic[path].Write(line);
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="callback">异步异常处理回调</param>
        public FileWriteManger(Action<Exception> callback)
        {
            _exceptionCallback = callback;
            Task.Factory.StartNew(CycleFlush, _mainToken.Token).
                ContinueWith(exitTask =>
                    {
                        if (!exitTask.IsFaulted) return;
                        if (_exceptionCallback == null) return;
                        _exceptionCallback(new Exception("日志写入线程异常退出,", exitTask.Exception));
                    });
        }

        /// <summary>
        /// 释放线程
        /// </summary>
        public void Dispose()
        {
            if (_mainToken != null)
            {
                _mainToken.Cancel();
                _mainToken.Dispose();
                _mainToken = null;
            }
            if (_threadWait == null) return;
            _threadWait.Set();
            _threadWait.Close();
            _threadWait.Dispose();
            _threadWait = null;
        }
    }

    /// <summary>
    /// 文件写入实体句柄
    /// </summary>
    public class FileWriter : IDisposable
    {
        //private readonly System.Text.StringBuilder _caches = new System.Text.StringBuilder();

        private Queue<string> _caches = new Queue<string>(1024);

        /// <summary>
        /// 缓冲字符串长度
        /// </summary>
        public int CachesCount { get { return _caches.Count; } }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary>
        /// 文件句柄
        /// </summary>
        //private StreamWriter _write;

        /// <summary>
        /// 最后一次活动时间
        /// </summary>
        public DateTime LastActivityTime { get; private set; }

        /// <summary>
        /// 写入数据到硬盘
        /// </summary>
        /// <param name="line">写入的数据</param>
        public void Write(string line)
        {
            lock (_caches)
                _caches.Enqueue(line);
            // 记录或动时间
            LastActivityTime = DateTime.Now;
        }

        /// <summary>
        /// 将文件写入内存
        /// </summary>
        public void Flush()
        {
            if (_caches.Count == 0) return;
            string[] lines;
            lock (_caches)
            {
                lines = _caches.ToArray();
                _caches.Clear();
            }
            // 判断是否创建目录
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
            try
            {
                File.AppendAllLines(FilePath, lines);
            }
            catch
            {
                // 将数据重新添加回缓存，等待下次写入
                lock(_caches)
                {
                   foreach(var line in lines)
                   {
                       _caches.Enqueue(line);
                   }
                }
                throw;
            }
        }

        private readonly string _directory;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="filePath">文本路径</param>
        public FileWriter(string filePath)
        {
            FilePath = filePath;
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Directory == null)
            {
                throw new InvalidOperationException(string.Format("invalid directory! {0}", filePath));
            }
            // 记录目录
            _directory = fileInfo.Directory.FullName;
            //_write =
            //    new StreamWriter(new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.Read),
            //                     System.Text.Encoding.Default, 4 * 1024);
            LastActivityTime = DateTime.Now;
        }

        /// <summary>
        /// 是否文件句柄
        /// </summary>
        public void Dispose()
        {
            //if (_write == null) return;
            //_write.Close();
            //_write.Dispose();
            //_write = null;

            if (_caches != null)
                _caches.Clear();
            _caches = null;

            GC.SuppressFinalize(this);
        }
    }
}
