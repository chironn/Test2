using System.Linq;
using System.Collections.Generic;
using DES.Core;
using DES.Core.Interfaces;

namespace DES.Converts.BYDQService
{
    /// <summary>
    /// 
    /// </summary>
    public class MainService : BaseService
    {
        #region Overrides of BaseService

        private readonly List<string> _filtrateList = new List<string>
            {
                "BYD企标下行数据接收队列",
                "BYD企标上行数据接收队列",
                "BYD企标上行数据发布队列",
                "BYD企标下行数据发布队列",
            };

        protected override IEnumerable<IQueueConfig> FiltrateConfig(IEnumerable<IQueueConfig> source)
        {
            return source.Where(config => _filtrateList.Contains(config.FindKey));
        }

        public override string ModuleName
        {
            get { return "DES.Converts.BYDQService"; }
        }
        
        #endregion
    }
}
