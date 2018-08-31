/* ==============================================================================
* 类型名称：Register  
* 类型描述：类型注册基类，用于完成类型或接口的注册
* 创 建 者：linfk
* 创建日期：2017/11/16 9:54:06
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

namespace DES.Core
{
    /// <summary>
    /// 类型注册基类，用于完成类型或接口的注册
    /// </summary>
    public class Register
    {

        /// <summary>
        /// 利用反射加载程序集里的协议类型和解析类型工厂
        /// </summary>
        /// <typeparam name="TEntity">搜索的接口类型</typeparam>
        /// <param name="assembly">加载的程序集</param>
        /// <returns>创建的实体对象</returns>
        public static List<TEntity> ReflectInstance<TEntity>(Assembly assembly)
        {
            var result = new List<TEntity>();
            // 反射解析器
            foreach (var instance in assembly.GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface && typeof(TEntity).IsAssignableFrom(type))
                .Select(find => (TEntity)Activator.CreateInstance(find)))
            {
                result.Add(instance);
            }

            return result;
        }

        /// <summary>
        /// 从当前程序域中加载
        /// </summary>
        /// <param name="descriptionString">类型描述字符串,格式：“[程序集类名称],[类型名称]”</param>
        /// <returns>反射加载到的类型</returns>
        public static Type FindTypeByDescription(string descriptionString)
        {
            var splits = descriptionString.Split(',');
            if (splits.Length < 2)
            {
                throw new FormatException("descriptionString");
            }
            var dllPath = string.Format("{0}{1}.dll", AppDomain.CurrentDomain.BaseDirectory,
                                        splits[0]);
            var assembly = Assembly.Load(File.ReadAllBytes(dllPath));

            return assembly != null ? assembly.GetTypes().FirstOrDefault(find => find.Name.Equals(splits[1])) : null;
        }

        /// <summary>
        /// 接口实现类型映射列表
        /// </summary>
        public static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Type> TypeMap =
            new System.Collections.Concurrent.ConcurrentDictionary<Type, Type>();

        /// <summary>
        /// 单例实体映射表
        /// </summary>
        public static readonly Dictionary<Type, Object> InstanceMap = new Dictionary<Type, Object>();


        /// <summary>
        /// 注册接口的实现类型，避免手动编码错误
        /// </summary>
        /// <typeparam name="TInterface">单例接口</typeparam>
        /// <typeparam name="TImplemented">实现类型</typeparam>
        public static void RegisterType<TInterface, TImplemented>()
        {
            if (typeof(TImplemented).GetInterfaces().Contains(typeof(TInterface)))
                TypeMap.TryAdd(typeof(TInterface), typeof(TImplemented));
            else
                throw new ArgumentException("Not Implemented",
                                            string.Format("{0},{1}", typeof(TInterface), typeof(TImplemented)));

        }

        /// <summary>
        /// 获取运行时单例
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <returns>单例对象，未注册该类型则返回NULL</returns>
        public static T Single<T>() where T : class
        {
            T intance;
            if (!InstanceMap.ContainsKey(typeof(T)))
            {
                intance = New<T>();
                if (intance != null)
                    InstanceMap[typeof(T)] = intance;
            }
            else
                intance = InstanceMap[typeof(T)] as T;
            return intance;
        }

        /// <summary>
        /// 创建类型
        /// </summary>
        /// <typeparam name="T">类型接口</typeparam>
        /// <returns>创建新类型</returns>
        public static T New<T>() where T : class
        {
            return TypeMap.ContainsKey(typeof(T)) ? (T)Activator.CreateInstance(TypeMap[typeof(T)]) : null;
        }
    }
}
