/* ==============================================================================
* 类型名称：BytesConvertManager  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/12/15 10:35:04
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
#if !DEBUG
using DES.Utilities.Expends;
#endif

namespace DES.Utilities
{
    /// <summary>
    /// 字节数据转换
    /// </summary>
    public class BytesConvertManager
    {
        private readonly DataCachesTaskCollection<BytesLog> _taskCollection;

        /// <summary>
        /// 转换线程队列
        /// </summary>
        public DataCachesTaskCollection<BytesLog> ConvertTaskCollection { get { return _taskCollection; } }

        private readonly Action<string, string> _logOutputCallback;
        /// <summary>
        /// 字节实体完成端口缓存池
        /// </summary>
        private readonly System.Collections.Concurrent.ConcurrentQueue<BytesLog> _byteslogPool =
            new System.Collections.Concurrent.ConcurrentQueue<BytesLog>();
        /// <summary>
        /// 从缓冲池获取一个实体，如果缓冲池数据不足，则创建一个实体
        /// </summary>
        /// <returns>实体对象</returns>
        private BytesLog GetOrCreateEntity()
        {
            BytesLog entity;
            return _byteslogPool.TryDequeue(out entity) ? entity : new BytesLog();
        }
        /// <summary>
        /// 日志转换处理
        /// </summary>
        /// <param name="log"></param>
        private void Dowork(BytesLog log)
        {
            // 生成字符串
            var line = log.Time.ToString("HH:mm:ss:fff") + "#" + log.GetString();
            var path = log.Path;
            // 还池
            _byteslogPool.Enqueue(log);
            // 输出字符串
            if (_logOutputCallback != null)
                _logOutputCallback(path, line);
        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logOutputCallback">日志文本输出回调</param>
        /// <param name="callback">异常处理回调</param>
        public BytesConvertManager(Action<string, string> logOutputCallback, Action<Exception> callback)
        {
            _logOutputCallback = logOutputCallback;
            _taskCollection = new DataCachesTaskCollection<BytesLog>(log => log.Path, Dowork, callback, 4);
        }

        /// <summary>
        /// 压入一包需要转换的数据
        /// </summary>
        /// <param name="path">转换后写入文件的路径</param>
        /// <param name="buffer">需要转换的字节数据</param>
        /// <param name="remarkString">添加在日志前端的扩展信息</param>
        public void Push(string path, byte[] buffer, string remarkString = "")
        {
            var entity = GetOrCreateEntity();
            entity.Initialize(DateTime.Now, buffer);
            entity.RmarkString = remarkString;
            entity.Path = path;
            _taskCollection.Push(entity);
        }
    }

    /// <summary>
    /// 日志日志实体
    /// </summary>
    public class BytesLog
    {
        /// <summary>
        /// log日志路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 其他描述信息，将放在字节数据包头
        /// </summary>
        public string RmarkString { get; set; }


        /// <summary>
        /// 日志报文时间
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// 日志字节长度
        /// </summary>
        private int _length;

        /// <summary>
        /// 日志字节数据
        /// </summary>
        private byte[] _buffer;

        /// <summary>
        /// 初始化字节数据
        /// </summary>
        /// <param name="time">日志数据时间</param>
        /// <param name="buffer">日志字节缓存</param>
        public void Initialize(DateTime time, byte[] buffer)
        {
            Time = time;
            // 当前字节数组长度小于初始化长度
            if (_buffer == null || _buffer.Length < buffer.Length)
            {
                Array.Resize(ref _buffer, buffer.Length);
            }
            // 字节长度
            _length = buffer.Length;
            // 短字节使用for循环复制效率较高。
            if (_length < 100)
            {
                for (var index = 0; index < _length; index++)
                    _buffer[index] = buffer[index];
            }
            else
            {
                Array.Copy(buffer, 0, _buffer, 0, _length);
            }

        }

        /// <summary>
        /// 将字节数据转换为16进制字符串，此操作消耗性能
        /// </summary>
        /// <returns>16进制字符串</returns>
        public string GetString()
        {
            if (_length == 0 || _buffer == null) return string.Empty;
#if DEBUG
            return BitConverter.ToString(_buffer, 0, _length).Replace("-", string.Empty);
#else 
            return RmarkString + _buffer.BytesToString(0, _length);
#endif

        }

    }
}
