/* ==============================================================================
* 类型名称：BytesLogWriteManger  
* 类型描述：字节日志写入服务
* 创 建 者：linfk
* 创建日期：2017/12/15 11:47:37
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

using System;
using System.Linq;
using System.Net;
using System.Configuration;
namespace DES.Utilities.IO
{
    /// <summary>
    /// 字节日志写入服务
    /// </summary>
    public class BytesLogWriteManger
    {
        #region 远程日志
        /// <summary>
        /// 是否启用远程日志
        /// </summary>
        public bool IsRomte
        {
            get { return _romteEp != null && _udpClient != null; }
        }


        /// <summary>
        /// 组播地址与端口
        /// </summary>
        private readonly IPEndPoint _romteEp;
        private readonly System.Net.Sockets.UdpClient _udpClient;
        public void RometeWrite(string line)
        {
            if (_romteEp == null) return;
            // 打印调试信息速度慢
            var buffer = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(line);
            _udpClient.Send(buffer, buffer.Length, _romteEp);
        }
        #endregion

        #region 文本写入

        public FileWriteManger FileWrite { get; private set; }

        /// <summary>
        /// 全局日志转换操作
        /// </summary>
        public BytesConvertManager ConvertManager { get; private set; }
        #endregion

        #region 配置读取
        private static string GetAppSettings(string key)
        {
            return ConfigurationManager.AppSettings.AllKeys.Contains(key) ? ConfigurationManager.AppSettings[key] : string.Empty;
        }

        private static bool GetAppSettings(string key, out int number)
        {
            number = 0;
            return ConfigurationManager.AppSettings.AllKeys.Contains(key) &&
                   int.TryParse(ConfigurationManager.AppSettings[key], out number);
        }
        #endregion

        #region 属性成员
        /// <summary>
        /// 是否禁用功能
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 日志的根目录
        /// </summary>
        public string Root { get; set; }

        /// <summary>
        /// 文件的后缀名
        /// </summary>
        public string Suffix { get; set; }
        #endregion

        #region 构造方法
        public BytesLogWriteManger()
        {
            var ip = GetAppSettings("远程日志IP");
            int number;
            if (!string.IsNullOrEmpty(ip))
            {
                if (GetAppSettings("远程日志端口", out number))
                {
                    _romteEp = new IPEndPoint(IPAddress.Parse(ip), number);
                    _udpClient = new System.Net.Sockets.UdpClient();
                }
            }
            FileWrite = new FileWriteManger(ExceptionHander);
            ConvertManager = new BytesConvertManager(Write, ExceptionHander);
            if (GetAppSettings("禁用报文日志", out number) && number == 1)
            {
                IsEnable = false;
            }
            else
            {
                IsEnable = true;
            }
            Root = "logs";
            Suffix = ".log";

        }

        #endregion

        #region 日志写入
        public string GetPath(string identity)
        {
            var date = DateTime.Now.ToString("yyyy-MM-dd");

            return AppDomain.CurrentDomain.BaseDirectory + @"\" + Root + @"\" + identity + @"\" + date + Suffix;


        }

        /// <summary>
        /// 写入字节数据
        /// </summary>
        /// <param name="identity">数据标识</param>
        /// <param name="buffer">字节数据</param>
        /// <param name="remark">其他信息</param>
        public void Write(string identity, byte[] buffer, string remark = "")
        {
            if (IsEnable || IsRomte)
            {
                ConvertManager.Push(GetPath(identity), buffer, remark);
            }
        }

        /// <summary>
        /// 写入一行数据
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="message">消息</param>
        public void Write(string path, string message)
        {
            if (IsRomte)
                RometeWrite(string.Format("{0}|{1}", path, message));
            if (!IsEnable) return;
            FileWrite.Push(path, message);
        }
        #endregion

        #region 异常处理

        public Action<Exception> ExceptoinCallback;

        private void ExceptionHander(Exception ex)
        {
            if (ExceptoinCallback != null)
            {
                ExceptoinCallback(new Exception("日志记录模块异常", ex));
            }
            else
                Console.WriteLine(ex);
        }

        #endregion
    }
}
