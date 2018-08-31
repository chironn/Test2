using System;
using System.Collections.Generic;
using System.Linq;
using DES.DbCaches.Implementeds;
using DES.DbCaches.Interfaces;

namespace DES.DbCaches
{
    /// <summary>
    /// 配置缓存操作方法封装
    /// <remarks>
    /// 设计目的：
    /// 1.分离数据缓存与业务逻辑实体，降低耦合
    /// 2.对上层业务逻辑实体提供统一缓存数据访问接口。
    /// 建议后续配置缓存开发在此基础上进行。
    /// </remarks>
    /// </summary>
    public abstract class Provider
    {
        #region Static Instance
        /// <summary>
        /// 静态构造，完成单例接口类型注册
        /// </summary>
        static Provider()
        {
            RegisterType<IDbcMapProvider, DbcMapProvider>();
            RegisterType<IDbProvider, DapperDbProvider>();
            RegisterType<IDbHExchangeLoad, DapperDbProvider>();
            RegisterType<IFaultMapOperator, FaultMapOperator>();
            RegisterType<IVehicleBaseInfoLoad, VehicleBaseInfoCaches>();
            RegisterType<IDbBYDQBaseInfoLoad, DapperDbProvider>();
            // RegisterType<IFaultMapOperators, FaultMapOperator>();
        }


        /// <summary>
        /// 接口实现类型映射列表
        /// </summary>
        private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Type> TypeMap =
            new System.Collections.Concurrent.ConcurrentDictionary<Type, Type>();


        /// <summary>
        /// 单例实体映射表
        /// </summary>
        private static readonly Dictionary<Type, Object> InstanceMap = new Dictionary<Type, Object>();

        public static T FactoryCreate<T>() where T : class
        {
            return TypeMap.ContainsKey(typeof(T)) ? (T)Activator.CreateInstance(TypeMap[typeof(T)]) : null;
        }

        /// <summary>
        /// 获取运行时单例
        /// </summary>
        /// <typeparam name="T">接口类型</typeparam>
        /// <returns>单例对象，未注册该类型则返回NULL</returns>
        public static T Intance<T>() where T : class
        {
            T intance;
            if (!InstanceMap.ContainsKey(typeof(T)))
            {
                intance = FactoryCreate<T>();
                if (intance != null)
                    InstanceMap[typeof(T)] = intance;
            }
            else
                intance = InstanceMap[typeof(T)] as T;
            return intance;
        }

        /// <summary>
        /// 注册单例接口的实现类型，避免手动编码错误
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

        #endregion
    }
}
