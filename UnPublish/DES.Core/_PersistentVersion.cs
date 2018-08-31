/* ==============================================================================
* 类型名称：Global  
* 类型描述：用于存储由编码时写入的常量信息，不允许在运行时进行修改的数据
* 创 建 者：linfk
* 创建日期：2018/1/18 13:03:53
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/

namespace DES.Core
{
    /// <summary>
    /// 全局静态字符串描述信息
    /// </summary>
    public sealed class _PersistentVersion
    {
        #region 版本信息生成
        static _PersistentVersion()
        {

            var V_1_9 = new _PersistentVersion("1.9")
            {
                Description =
                    "1.DES.Converts.Gbt32960，去掉使用极值判断数据是否分帧功能，采用单体电池数量判断数据是否分帧" + "\r\n"
                    + "2.DES.Converts.Gbt32960 提供动态加载数据过滤插件功能，插件默认存放在应用程序目录‘plugins’下面" + "\r\n"
                    + "3.‘plugins’目录下添加‘DES.ConvertsFilters.NJJL.dll’文件，则可支持南京金龙项目要求的过滤无效里程值的功能" + "\r\n"
            };
            Versions.Add(V_1_9, V_1_9);

            var V_1_8 = new _PersistentVersion("1.8")
                {
                    Description =
                        "1.DES.Converts.Gbt32960，修复‘1.7’版本当未配置里程数据是，转换结果为‘0xFFFF’不为国标规定的‘0xFFFFFFFF’的bug。" + "\r\n"
                        + "2.DES.Converts.Gbt32960 提供北汽宁德历史数据抽取转换功能。" + "\r\n"
                };
            Versions.Add(V_1_8, V_1_8);


            var V_1_7 = new _PersistentVersion("1.7")
            {
                Description =
                     "1.为 socket 通信方式提供框架加载支持。" + "\r\n"
                   + "2.DES.Converts.Gbt32960 提供了分帧逻辑处理。" + "\r\n"
                   + "3.DES.FileWriters.Gbt32960 支持深圳历史文本导出。" + "\r\n"
            };
            Versions.Add(V_1_7, V_1_7);




            var V_1_6 = new _PersistentVersion("1.6")
                {
                    Description =
                         "1.提供配置文件启动功能。" + "\r\n"
                       + "2.DES.Converts.Gbt32960 程序支持故障数据转换。" + "\r\n"
                       + "3.支持版本信息打印。" + "\r\n"
                };

            Versions.Add(V_1_6, V_1_6);

            Version = Versions[V_1_9];
        }
        #endregion

        public static readonly System.Collections.Generic.Dictionary<string, _PersistentVersion> Versions = new System.
            Collections.Generic.Dictionary<string, _PersistentVersion>();

        /// <summary>
        /// 全局版本信息
        /// </summary>
        public static readonly _PersistentVersion Version;

        #region 构造方法
        private readonly string _contextString;

        public string Description { get; private set; }

        private _PersistentVersion(string context)
        {
            _contextString = context;
        }

        public override string ToString()
        {
            return _contextString;
        }

        #endregion

        #region 扩展方法
        public static implicit operator string(_PersistentVersion version)
        {
            return version._contextString;
        }
        #endregion
    }
}
