
using System.IO;
using System.Linq;
using System.Reflection;
using DES.Core;
using log4net;
using log4net.Appender;

namespace DES.InLog
{
    public class InLogFactory : LogProvider
    {

        protected virtual void OnIntitLog(BaseService service)
        {
            inCom.Logs.LogProvider.Create().InitLog4Net(service.ConfigFactory.LoadConfig("log4net.config"));
            if (string.IsNullOrEmpty(service.BaseLogDirecotory)) return;
            var storedPath = LogManager.GetRepository();
            var appenders = storedPath.GetAppenders();
            foreach (var ra in appenders.OfType<RollingFileAppender>())
            {
                var path = Directory.GetParent(ra.File).FullName;
                ra.File = Path.Combine(path, service.BaseLogDirecotory);
                ra.ActivateOptions();
            }
        }

        protected virtual void OnIntitLog(string config)
        {
            inCom.Logs.LogProvider.Create().InitLog4Net(config);
        }

        public override Core.Interfaces.ILogWrite GetLog(string context)
        {
            inCom.Logs.LogProvider.Create().InitLog4Net(new FileInfo(@"IniFiles\log4net.config"));
            return new InLogWrite(context);
        }

        public override Core.Interfaces.ILogWrite GetLog(string context, BaseService service)
        {
            OnIntitLog(service);
            return new InLogWrite(context);
        }

        public override Core.Interfaces.ILogWrite GetLog(string context, string config)
        {
            OnIntitLog(config);
            return new InLogWrite(context);
        }
    }
}
