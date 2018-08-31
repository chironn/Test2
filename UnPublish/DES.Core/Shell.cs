/* ==============================================================================
* 类型名称：Shell  
* 类型描述：
* 创 建 者：linfk
* 创建日期：2017/12/8 9:28:31
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DES.Core
{
    /// <summary>
    /// DIS 程序外壳封装
    /// </summary>
    public class Shell
    {
        /// <summary>
        /// 当前外壳程序中运行的服务
        /// </summary>
        public static BaseService Service { get; private set; }
        public static Shell Wrap { get; private set; }

        /// <summary>
        /// 程序入口
        /// </summary>
        /// <param name="service">需要运行的服务</param>
        /// <param name="args">控制台参数</param>
        /// <param name="wrap">服务外壳</param>
        /// <exception cref="Exception">本方法不捕获内部异常</exception>
        public static void Run(BaseService service, string[] args = null, Shell wrap = null)
        {
            if (Service != null)
            {
                throw new InvalidOperationException("service is runing");
            }
            Wrap = wrap;
            Service = service;
            if (Wrap == null) Wrap = new Shell();
            Wrap.ConsoleCommandExecuteEvent += Wrap_ConsoleCommandExecuteEvent;
            Wrap.ConsoleCommandQueryEvent += Wrap_ConsoleCommandQueryEvent;
            Wrap.Start(service, args);
        }

        static void Wrap_ConsoleCommandQueryEvent(List<string> obj)
        {
            obj.Add("exit:主程序退出");
        }



        static void Wrap_ConsoleCommandExecuteEvent(Shell shell, string obj)
        {
            if (obj.Trim().ToLower() != "exit") return;
            Process.GetCurrentProcess().Kill();
        }


        protected virtual void OnAppearanceInitialize(string[] args)
        {
            if (Environment.UserInteractive)
            {
                Console.Title = string.Format("[{0}]", Process.GetCurrentProcess().Id);
                if (ConfigurationManager.AppSettings.AllKeys.Contains("服务显示名"))
                {
                    Console.Title += ConfigurationManager.AppSettings["服务显示名"];
                }
                else
                {
                    var middleTile = string.Empty;
                    if (args != null && args.Length > 0)
                    {
                        var filePath = args[0];
                        if (File.Exists(filePath))
                        {
                            Console.WriteLine("文件启动:{0}", filePath);
                            var file = new FileInfo(filePath);
                            middleTile = file.Name;
                        }
                    }

                    if (string.IsNullOrEmpty(middleTile))
                    {
                        var directory = AppDomain.CurrentDomain.BaseDirectory;
                        var parent =
                            directory.Split(Path.DirectorySeparatorChar).Last(find => !string.IsNullOrEmpty(find));
                        middleTile = string.Format("{0}/{1}", parent, Process.GetCurrentProcess().ProcessName);
                    }
                    Console.Title += middleTile;
                }

                Console.Title += "_Ver " + _PersistentVersion.Version;
            }
        }

        /// <summary>
        /// 控制台命令执行时间，当界面输入控制台字符串时将产生此回调
        /// </summary>
        public event Action<Shell, string> ConsoleCommandExecuteEvent;
        /// <summary>
        /// 命令查询执行时触发事件，将需要输出的内容添加到输入的列表里，在调用结束后会自动打印
        /// </summary>
        public event Action<List<string>> ConsoleCommandQueryEvent;

        private readonly System.Threading.CancellationTokenSource _mainCancelTokenSource =
            new System.Threading.CancellationTokenSource();

        private static void Write(string message, ConsoleColor changeColor)
        {
            var color = Console.ForegroundColor;

            Console.ForegroundColor = changeColor;
            Console.WriteLine(message);
            Console.ForegroundColor = color;
        }

        /// <summary>
        /// 使用控制台分块打印多行输出
        /// </summary>
        /// <param name="lines">需要打印的数据内用</param>
        public void ConsolePrintLines(List<string> lines)
        {
            var lineCount = 0;
            var maxline = 15;
            foreach (var str in lines)
            {
                if (lineCount > maxline && lines.Count > lineCount)
                {
                    Write(string.Format("还有剩余:{0} 行内容,输入任意键获取更多内容，数据‘all’打印剩下所有内容", lines.Count - lineCount),
                          ConsoleColor.Red);
                    var line = Console.ReadLine();
                    if (!string.IsNullOrEmpty(line) && line.ToLower().Equals("all"))
                    {
                        maxline = lines.Count;
                    }
                    maxline += 15;
                }
                Write(str, ConsoleColor.Green);
                lineCount++;
            }
        }

        public void ConsolePrintLine(string line)
        {
            Write(line, ConsoleColor.Green);
        }

        protected virtual void OnCmdRead()
        {
            while (!_mainCancelTokenSource.IsCancellationRequested)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                {
                    Write("输入 '?' 查询指令列表", ConsoleColor.Red);
                    continue;
                }
                if (line.Trim().Equals("?"))
                {
                    var lines = new List<string>();
                    if (ConsoleCommandQueryEvent != null)
                        ConsoleCommandQueryEvent(lines);
                    if (lines.Count == 0)
                    {
                        Write("无控制指令订阅!", ConsoleColor.Red);
                    }
                    ConsolePrintLines(lines);
                    continue;
                }
                if (ConsoleCommandExecuteEvent == null) continue;
                try
                {
                    ConsoleCommandExecuteEvent(this, line);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("【{0}】命令执行异常：{1}", line, ex);
                }

            }
        }


        protected virtual void OnServiceRun(BaseService service, string[] args = null)
        {
            if (args != null && args.Length > 0)
            {
                var filePath = args[0];
                if (File.Exists(filePath))
                {
                    var file = new FileInfo(filePath);
                    service.BaseLogDirecotory = file.Name.Replace('.', '_') + @"\";
                    service.CommunicationContent = File.ReadAllText(filePath);
                }
            }
            service.Run();
        }

        public virtual void Start(BaseService service, string[] args = null)
        {
            OnAppearanceInitialize(args);
            OnServiceRun(service, args);
            OnCmdRead();
        }
    }
}
