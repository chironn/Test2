/* ==============================================================================
* 类型名称：BaseService  
* 类型描述：框架服务基类
* 创 建 者：linfk
* 创建日期：2017/11/16 9:49:49
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using DES.Core.Diagnostics;
using DES.Core.Interfaces;

namespace DES.Core
{
    /// <summary>
    /// 框架服务基类
    /// </summary>
    public abstract class BaseService
    {
        #region 诊断器
        private readonly InvokeWatcherManager _diagnosticsWatch = new InvokeWatcherManager();
        /// <summary>
        /// 诊断器
        /// </summary>
        public virtual InvokeWatcherManager DiagnosticsWatch { get { return _diagnosticsWatch; } }

        #endregion

        #region 外部输入
        /// <summary>
        /// 设置启动的通信配置文件内容
        /// </summary>
        public string CommunicationContent { get; set; }

        /// <summary>
        /// 服务日志存放目录
        /// </summary>
        public string BaseLogDirecotory { get; set; }
        #endregion

        #region 日志操作
        protected ILogWrite FrameworkLogWrite { get; private set; }
        public virtual void WriteInfo(string message)
        {
            FrameworkLogWrite.WriteInfo(message);
        }
        public virtual void WriteWarn(string message)
        {
            FrameworkLogWrite.WriteWarn(message);
        }
        public virtual void WriteError(string message)
        {
            FrameworkLogWrite.WriteError(message);
        }
        public virtual void WriteError(Exception ex, string message = "")
        {
            FrameworkLogWrite.WriteError(ex, message);
        }
        public virtual void WriteErrorCode(string errorCode, string append, params object[] param)
        {
            FrameworkLogWrite.Write(errorCode, append, param);
        }
        #endregion

        #region 统计计数
        /// <summary>
        /// 用于“Zabbix”计数的字典信息
        /// key 如:处理速度 value 如:5000
        /// </summary>
        public System.Collections.Concurrent.ConcurrentDictionary<string, string> ZabbixDic { get; private set; }

        protected long ReceiveCount;

        protected long PublishMessageCount;
        /// <summary>
        /// 本模块异步线程取消信号
        /// </summary>
        protected readonly System.Threading.CancellationTokenSource _mainCancelToken =
            new System.Threading.CancellationTokenSource();
        /// <summary>
        /// 通信队列IO 在 Zabbix 更新操作线程
        /// </summary>
        protected void UpdataZabbixThread()
        {
            var watcher = new Stopwatch();
            // 10 秒统计一次
            const int sleepMilliseconds = 10 * 1000;
            var lastElapseMilliseconds = 0.0;
            while (!_mainCancelToken.IsCancellationRequested)
            {

                watcher.Restart();
                try
                {
                    IOZabbixUpdate(lastElapseMilliseconds / 1000);
                    OnZabbixUpdate(lastElapseMilliseconds / 1000);
                }
                catch (Exception ex)
                {
                    WriteError(ex, "主框架 Zabbix 刷新线程未捕获异常");
                }
                watcher.Stop();
                // 记录本次操作时长
                lastElapseMilliseconds = sleepMilliseconds + watcher.Elapsed.TotalMilliseconds;

                // 线程每次休眠1秒
                System.Threading.Thread.Sleep(sleepMilliseconds);
            }
        }

        protected const string RECEIVECOUNT = "通信总线 接收 数据总条数";
        protected const string SENDCOUNT = "通信总线 发送 数据总条数";
        protected const string RECEIVERATE = "通信总线 接收 数据速率(条/秒)";   
        protected const string SENDERATE = "通信总线 发送 数据速率(条/秒)";

        protected long LastReceiveCount = 0;
        protected long LastPublishMessageCount = 0;
        /// <summary>
        /// 更新 zabbix 状态
        /// </summary>
        /// <param name="lastElapseSeconds">上次更新时长</param>
        protected void IOZabbixUpdate(double lastElapseSeconds)
        {
            if (!ZabbixDic.ContainsKey(RECEIVECOUNT))
            {
                ZabbixDic[RECEIVECOUNT] = "0";
            }
            if (!ZabbixDic.ContainsKey(RECEIVERATE))
            {
                ZabbixDic[RECEIVERATE] = "0";
            }

            if (!ZabbixDic.ContainsKey(SENDCOUNT))
            {
                ZabbixDic[SENDCOUNT] = "0";
            }
            if (!ZabbixDic.ContainsKey(SENDERATE))
            {
                ZabbixDic[SENDERATE] = "0";
            }

            // 统计总数
            ZabbixDic[RECEIVECOUNT] = ReceiveCount.ToString(CultureInfo.InvariantCulture);
            ZabbixDic[SENDCOUNT] = PublishMessageCount.ToString(CultureInfo.InvariantCulture);



            if (Math.Abs(lastElapseSeconds - 0) > double.Epsilon)
            {
                // 统计速率
                ZabbixDic[RECEIVERATE] = string.Format("{0:0.00}", (ReceiveCount - LastReceiveCount) / lastElapseSeconds);
                ZabbixDic[SENDERATE] = string.Format("{0:0.00}", (PublishMessageCount - LastPublishMessageCount) / lastElapseSeconds);
            }

            // 记录本次数量
            LastReceiveCount = ReceiveCount;
            LastPublishMessageCount = PublishMessageCount;
        }

        /// <summary>
        /// 更新Zabbix操作，父类为子类提供的线程调用入口
        /// </summary>
        /// <param name="lastElapseSeconds">距离上次调用的时长，单位:秒</param>
        protected virtual void OnZabbixUpdate(double lastElapseSeconds)
        {

        }

        /// <summary>
        /// 队列吞吐量打印操作
        /// </summary>
        protected void OnUpdataZabbixIOZabbix()
        {
            WriteInfo("Zabbix 监控 MQ通信IO 线程启动！");
            System.Threading.Tasks.Task.Factory.StartNew(UpdataZabbixThread, _mainCancelToken.Token);
        }


        #endregion

        #region 配置文件读取

        #endregion

        #region 配置工厂初始化
        protected virtual ConfigProvider OnCreateConfigFactory()
        {
            var content = ConfigurationManager.AppSettings.AllKeys.Contains("ConfigFactory")
                              ? ConfigurationManager.AppSettings["ConfigFactory"]
                              : string.Empty;
            if (string.IsNullOrEmpty(content))
            {
                return new ConfigProvider();
            }
            var type = Register.FindTypeByDescription(content);
            return Activator.CreateInstance(type) as ConfigProvider;
        }
        public ConfigProvider ConfigFactory { get; protected set; }
        protected virtual void OnConfigProviderInitialize()
        {
            ConfigFactory = OnCreateConfigFactory();
        }
        #endregion

        #region 日志工厂初始化
        protected virtual LogProvider OnCreateLogFactory()
        {
            var content = ConfigurationManager.AppSettings.AllKeys.Contains("LogFactory")
                              ? ConfigurationManager.AppSettings["LogFactory"]
                              : string.Empty;

            var type = Register.FindTypeByDescription(content);
            return Activator.CreateInstance(type) as LogProvider;
        }
        public LogProvider LogFactory { get; protected set; }
        protected virtual void OnLogProviderInitialize()
        {
            LogFactory = OnCreateLogFactory();
            FrameworkLogWrite = LogFactory.GetLog(ModuleName, this);
        }
        #endregion

        #region 通信工厂初始化

        /// <summary>
        /// 从配置文件中过滤自己使用的配置通道信息
        /// </summary>
        /// <param name="source">原配置文件</param>
        /// <returns>过滤后的配置文件</returns>
        protected abstract IEnumerable<IQueueConfig> FiltrateConfig(IEnumerable<IQueueConfig> source);

        /// <summary>
        /// 本模块内数据发送队列
        /// </summary>
        protected readonly System.Collections.Concurrent.ConcurrentDictionary<string, IPushQueue> PushQueueDic =
            new System.Collections.Concurrent.ConcurrentDictionary<string, IPushQueue>();

        /// <summary>
        /// 通信异常消息处理
        /// </summary>
        /// <param name="queue">消息队列</param>
        /// <param name="ex">异常</param>
        /// <param name="append">附加信息</param>
        protected virtual void OnCommunicationException(ICommunicationQueue queue, Exception ex, string append)
        {
            FrameworkLogWrite.WriteError(ex, string.Format("通信队列:{0},异常,信息 :{1}", queue.FindKey,
                                                       append ?? "未知"));
        }

        /// <summary>
        /// 从通信实体转换为逻辑实体
        /// </summary>
        /// <param name="entity">通信实体</param>
        /// <returns>逻辑实体</returns>
        protected virtual ILogicEntity OnConvertToLogic(ICommunicationEntity entity)
        {
            return LogicFactory.ParseToLogic.ContainsKey(entity.FindKey) ?
                         LogicFactory.ParseToLogic[entity.FindKey].Convert(entity) :
                         LogicFactory.DefaultParseLogic.Convert(entity);
        }

        /// <summary>
        /// 执行对应逻辑指令
        /// </summary>
        /// <param name="logic">逻辑操作</param>
        /// <param name="entity">通信实体</param>
        /// <param name="logicEntity">逻辑实体</param>
        protected virtual void Dowork(ILogicWork logic, ICommunicationEntity entity, ILogicEntity logicEntity)
        {
            logic.Dowork(entity, logicEntity);

        }

        /// <summary>
        /// 数据接收处理
        /// </summary>
        /// <param name="entity">数据实体</param>
        protected virtual void OnReceive(ICommunicationEntity entity)
        {
            if (ReceiveCount == long.MaxValue)
            {
                WriteInfo(string.Format("数据接收计数器达到最大值，计数器清空"));
                ReceiveCount = 0;
            }
            System.Threading.Interlocked.Increment(ref ReceiveCount);

            if (!LogicFactory.SubscribeDic.ContainsKey(entity.FindKey)) return;
            foreach (var logic in LogicFactory.SubscribeDic[entity.FindKey])
            {
                try
                {
                    var logicEntity = OnConvertToLogic(entity);

                    Dowork(logic, entity, logicEntity);
                }
                catch (Exception ex)
                {
                    FrameworkLogWrite.WriteError(ex, string.Format("逻辑处理队列:{0},运行任务时发生未捕获异常", logic.FindKey));
                }
            }
        }

        /// <summary>
        /// 数据通信工厂
        /// </summary>
        public CommunicationProvider CommunicateFactory { get; protected set; }
        protected virtual CommunicationProvider OnCreateCommunicateFactory()
        {
            var content = ConfigurationManager.AppSettings.AllKeys.Contains("CommunicationFactory")
                              ? ConfigurationManager.AppSettings["CommunicationFactory"]
                              : string.Empty;
            var type = Register.FindTypeByDescription(content);
            var result = Activator.CreateInstance(type) as CommunicationProvider;
            if (result != null)
                result.LogWrite = FrameworkLogWrite;
            return result;
        }

        /// <summary>
        /// 全局配置信息
        /// </summary>
        public readonly Dictionary<string, IQueueConfig> QueueConfigsDic = new Dictionary<string, IQueueConfig>();

        protected virtual List<IQueueConfig> OnCreateQueueConfig()
        {
            if (string.IsNullOrEmpty(CommunicationContent))
            {
                Console.WriteLine("本地配置文件启动");
                // 读取配置文件内容
                CommunicationContent = ConfigFactory.LoadConfig(CommunicateFactory.ConfigFileName);
            }

            var configs = CommunicateFactory.CreateConfig(CommunicationContent);

            // 记录全局配置信息
            foreach (var config in configs)
            {
                QueueConfigsDic[config.FindKey] = config;
            }

            return configs;
        }

        protected virtual void BeforePopQueueRun(IPopQueue popQueue)
        {
            if (popQueue == null) return;
            WriteInfo(string.Format("创建数据消费队列:{0},队列信息:{1}", popQueue.FindKey, popQueue.Config.ToString()));
            popQueue.ReceiveCallback = OnReceive;
        }

        protected virtual void BeforePushQueueRun(IPushQueue pushQueue)
        {
            if (pushQueue == null) return;
            WriteInfo(string.Format("创建数据生产队列:{0},队列信息:{1}", pushQueue.FindKey, pushQueue.Config.ToString()));
            PushQueueDic[pushQueue.FindKey] = pushQueue;
        }

        /// <summary>
        /// 队列启动前事件
        /// </summary>
        public event Action<BaseService, ICommunicationQueue> BeforeQueueRunEvent;

        protected virtual void BeforeQueueRun(ICommunicationQueue queue)
        {
            queue.ExcptionCallback = OnCommunicationException;

            // 数据接收队列
            BeforePopQueueRun(queue as IPopQueue);
            // 数据发送队列
            BeforePushQueueRun(queue as IPushQueue);

            if (BeforeQueueRunEvent == null) return;
            // 通知队列创建完成
            BeforeQueueRunEvent(this, queue);
        }

        protected virtual void OnCommunicateQueueInitialze(IQueueConfig config)
        {

            var queue = CommunicateFactory.CreateQueue(config);

            BeforeQueueRun(queue);

            // 启动通信队列
            CommunicateFactory.RunQueue(queue);
        }

        /// <summary>
        /// 数据通信工厂实例化
        /// </summary>
        protected virtual void OnCommunicateProviderInitialze()
        {
            // 创建工厂实体
            CommunicateFactory = OnCreateCommunicateFactory();

            // 根据配置信息，创建配置实体,并过滤
            var configs = FiltrateConfig(OnCreateQueueConfig());
            // 创建队列
            foreach (var config in configs)
            {
                OnCommunicateQueueInitialze(config);
            }
        }
        #endregion

        #region 逻辑工厂初始化
        // 发布消息
        public virtual void PublishMessageEvent(ILogicWork logic, ILogicEntity entity)
        {

            if (!LogicFactory.ParseToCommunication.ContainsKey(entity.FindKey))
            {
                WriteError(string.Format("逻辑:{0} 消息队列发布失败,未找到实体需要发布的解析实体:{1}",
                                         logic != null ? logic.GetType().Name : "null", entity.FindKey));
                return;
            }
            // 取池
            var communicationEntity = CommunicateFactory.GetOrCreateEntity(entity.FindKey);
            try
            {
                // 反解析
                LogicFactory.ParseToCommunication[entity.FindKey].Convert(communicationEntity, entity);
            }
            catch (Exception ex)
            {
                WriteError(string.Format("逻辑:{0} 逻辑数据[{1}] 反解失败,异常:{2}",
                                         logic != null ? logic.GetType().Name : "null", entity.FindKey, ex));
                // 还池,使用者还池
                //CommunicateFactory.GivebackEntity(communicationEntity);
                return;
            }

            if (!PushQueueDic.ContainsKey(entity.FindKey))
            {
                WriteError(string.Format("逻辑:{0} 消息队列发布失败,未找到实体需要发布的消息队列:{1}",
                                         logic != null ? logic.GetType().Name : "null", entity.FindKey));

                return;
            }
            try
            {
                // 推送数据
                PushQueueDic[entity.FindKey].Push(communicationEntity);
            }
            catch (Exception ex)// 传入参数由调用使用者归还
            {
                WriteError(ex, string.Format("逻辑:{0} 消息{1}发布失败",
                                             logic != null ? logic.GetType().Name : "null", entity.FindKey));

            }

            if (PublishMessageCount == long.MaxValue)
            {
                WriteInfo(string.Format("数据发送计数器达到最大值，计数器清空"));
                PublishMessageCount = 0;
            }
            // 计数器计数
            System.Threading.Interlocked.Increment(ref PublishMessageCount);
        }

        public LogicWorkProvider LogicFactory { get; protected set; }
        // 创建逻辑处理组件工厂
        protected virtual LogicWorkProvider OnCreateLogicFactory()
        {
            var content = ConfigurationManager.AppSettings.AllKeys.Contains("LogicFactory")
                              ? ConfigurationManager.AppSettings["LogicFactory"]
                              : string.Empty;

            if (string.IsNullOrEmpty(content))
            {
                return new LogicWorkProvider();
            }
            var type = Register.FindTypeByDescription(content);
            return Activator.CreateInstance(type) as LogicWorkProvider;
        }
        // 初始化逻辑处理组件
        protected virtual void OnLogicProviderInitialze()
        {
            LogicFactory = OnCreateLogicFactory();
            // 加载逻辑处理组件
            var logics = LogicFactory.LoadAssembly(this, GetType().Assembly);
            foreach (var logic in logics)
            {
                //注册控制台指令
                RegisterCmd(logic);
                logic.PublishMessageEvent += PublishMessageEvent;
                logic.Initialize();
                if (logic.SubscribeList != null && logic.SubscribeList.Count != 0)
                {
                    WriteInfo(string.Format("逻辑插件[{0}]订阅[{1}]通信消息", logic.FindKey, string.Join(",", logic.SubscribeList)));
                    continue;
                }
                WriteWarn(string.Format("逻辑组件:{0},类型:{1},未订阅消息队列", logic.FindKey, logic.GetType().Name));
            }
        }


        #endregion

        #region 服务启动
        /// <summary>
        /// 程序启动
        /// </summary>
        protected virtual void OnRun()
        {
            OnConfigProviderInitialize();
            OnLogProviderInitialize();
            OnLogicProviderInitialze();
            OnCommunicateProviderInitialze();
            OnUpdataZabbixIOZabbix();
            OnEventSubscribe();
        }

        /// <summary>
        /// 服务启动操作
        /// </summary>
        public void Run()
        {
            OnRun();
        }

        protected virtual void OnStop()
        {
            if (_mainCancelToken != null)
            {
                _mainCancelToken.Cancel();
                _mainCancelToken.Dispose();
                return;
            }

            if (CommunicateFactory == null) return;
            CommunicateFactory.DisposeQueue();
        }

        public void Stop()
        {
            OnStop();
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        public abstract string ModuleName { get; }
        #endregion

        #region 构造函数
        protected BaseService()
        {
            ZabbixDic = new System.Collections.Concurrent.ConcurrentDictionary<string, string>();
        }
        #endregion

        #region 命令事件相关

        protected Dictionary<string, CmdActionAttribute> CmdDic = new Dictionary<string, CmdActionAttribute>();
        /// <summary>
        /// 查询命令
        /// </summary>
        /// <param name="obj">用于返回信息的命令列表</param>
        protected void ConsoleCommandQueryEvent(List<string> obj)
        {
            var keys = CmdDic.Keys.Select(key => string.Format("{0}:{1}", key.ToLower(), CmdDic[key].Remark));
            obj.AddRange(keys);
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="shell">外壳程序</param>
        /// <param name="line">命令体</param>
        protected void ConsoleCommandExecuteEvent(Shell shell, string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            var splits = line.ToLower().Split(new[] { ' ', ':' });
            var cmd = splits[0].ToLower().Trim();
            if (!CmdDic.ContainsKey(cmd))
                shell.ConsolePrintLine(string.Format("未知指令:{0}", splits[0]));
            else
                CmdDic[cmd].DoCmdCallback(line, shell, this);
        }

        /// <summary>
        /// 事件订阅事件
        /// </summary>
        protected virtual void OnEventSubscribe()
        {
            if (Shell.Wrap == null) return;
            RegisterCmd(this);
            Shell.Wrap.ConsoleCommandExecuteEvent += ConsoleCommandExecuteEvent;
            Shell.Wrap.ConsoleCommandQueryEvent += ConsoleCommandQueryEvent;
        }

        public void RegisterCmd(Object entity)
        {
            //所有方法 
            foreach (var method in entity.GetType().GetMethods())
            {
                //找到方法对应的属性
                var attr = method.GetCustomAttributes(typeof(CmdActionAttribute), true).FirstOrDefault();
                var cmdAttr = attr as CmdActionAttribute;
                if (cmdAttr == null) continue;
                //从函数签名中创建一个委托,动态返回一个委托
                var doCmd = (DoCmd)Delegate.CreateDelegate(typeof(DoCmd), entity, method);
                //初始化单个参数处理集合
                cmdAttr.DoCmdCallback = doCmd;
                CmdDic.Add(cmdAttr.Cmd.ToLower().Trim(), cmdAttr);
            }
        }

        #endregion

        #region 控制命令
        [CmdActionAttribute("ver", "打印当前版本信息即说明,命令格式:ver:[版本号/all] 打印指定版本信息，或当所有历史版本信息")]
        public void PrintVersion(string param, Shell shell, BaseService servcie)
        {
            var splits = param.Split(new[] { ':', ',', ' ' });
            if (splits.Length < 2)
            {
                shell.ConsolePrintLine(string.Format("版本号:{0}", _PersistentVersion.Version));
                shell.ConsolePrintLine(_PersistentVersion.Version.Description);
            }
            else if (splits[1].ToLower().Trim().Equals("all"))
            {
                var lines = new List<string>();
                foreach (var version in _PersistentVersion.Versions.Values)
                {
                    lines.Add(string.Format("--------版本号:{0}--------", version));
                    lines.Add(version.Description);
                }
                shell.ConsolePrintLines(lines);
            }
            else
            {
                if (_PersistentVersion.Versions.ContainsKey(splits[1]))
                {
                    shell.ConsolePrintLine(string.Format("版本号:{0}", _PersistentVersion.Versions[splits[1]]));
                    shell.ConsolePrintLine(_PersistentVersion.Versions[splits[1]].Description);
                }
                else
                {
                    shell.ConsolePrintLine(string.Format("未找到版本编号为:[{0}] 的版本描述", splits[1]));
                }
            }
        }

        [CmdActionAttribute("io", "打印当前通信队列IO情况")]
        public void PrintIO(string param, Shell shell, BaseService servcie)
        {
            var lines = ZabbixDic.Select(pair => string.Format("{0},{1}", pair.Key, pair.Value)).ToList();
            shell.ConsolePrintLines(lines);
        }

        [CmdActionAttribute("queuecfg", "打印当前通信队列IO情况")]
        public void PrintQueueInfo(string param, Shell shell, BaseService servcie)
        {
            var lines = CommunicateFactory.RunQueueDic.Values.Select(queue => queue.Config.ToString()).ToList();
            shell.ConsolePrintLines(lines);
        }
        #endregion
    }
}
