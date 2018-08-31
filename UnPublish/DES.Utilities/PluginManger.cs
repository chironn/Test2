/* ==============================================================================
* 类型名称：PluginManger  
* 类型描述：插件管理类型
* 创 建 者：linfk
* 创建日期：2018/8/7 15:57:19
* =====================
* 修改者：
* 修改描述：
# 修改日期
* ==============================================================================*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DES.Utilities
{
    /// <summary>
    /// 插件管理类型
    /// </summary>
    public class PluginManger
    {
        /// <summary>
        /// 从指定目录中加载程序集
        /// </summary>
        /// <param name="directory">目录</param>
        /// <returns>程序集</returns>
        public static List<Assembly> LoadPlugins(string directory)
        {
            var result = new List<Assembly>();
            if (Directory.Exists(directory))
            {
                var directoryInfo = new DirectoryInfo(directory);
                result.AddRange(directoryInfo.GetFiles().Where(find => find.Extension.Equals(".dll")).Select(fileInfo => Assembly.Load(File.ReadAllBytes(fileInfo.FullName))));
            }
            return result;
        }


        /// <summary>
        /// 从指定路径下加载接口实体实例
        /// </summary>
        /// <typeparam name="TEntity">实体接口</typeparam>
        /// <param name="directory">插件文件路径</param>
        /// <returns>实体实例列表</returns>
        public static List<TEntity> LoadPlugins<TEntity>(string directory = "plugins")
        {
            var result = new List<TEntity>();

            if (Directory.Exists(directory))
            {
                var directoryInfo = new DirectoryInfo(directory);
                foreach (var fileInfo in directoryInfo.GetFiles().Where(find => find.Extension.Equals(".dll")))
                {
                    result.AddRange(LoadPlugins<TEntity>(Assembly.Load(File.ReadAllBytes(fileInfo.FullName))));
                }
            }

            return result;
        }

        /// <summary>
        /// 从指定路径下加载接口实体实例
        /// </summary>
        /// <typeparam name="TEntity">实体接口</typeparam>
        /// <param name="assembly">插件程序集</param>
        /// <returns>实体实例</returns>
        public static List<TEntity> LoadPlugins<TEntity>(Assembly assembly)
        {
            var result = new List<TEntity>();
            // 反射解析器
            foreach (var entity in assembly.GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface && typeof(TEntity).IsAssignableFrom(type))
                .Select(find => (TEntity)Activator.CreateInstance(find)))
            {
                result.Add(entity);
            }
            return result;
        }

    }



}
