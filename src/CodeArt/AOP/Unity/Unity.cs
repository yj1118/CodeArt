using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;

using CodeArt.Common;
using CodeArt.Concurrent.Sync;

namespace CodeArt.AOP
{
    /// <summary>
    /// 全局的，不提供会话级别的反转控制
    /// </summary>
    public static class Unity
    {

        public static void Inject<T>(string containerName, ContractImplement implement) where T : class
        {
            IUnityContainer container = GetOrCreateContainer(containerName);
            container.Inject<T>(implement);
        }

        public static void Inject<T>(ContractImplement implement) where T : class
        {
            Inject<T>("default", implement);
        }


        #region 得到对象实例

        /// <summary>
        /// 从容器<paramref name="containerName"/>中得到对象的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="containerName"></param>
        /// <returns></returns>
        public static T Resolve<T>(string containerName) where T : class
        {
            IUnityContainer container = GetContainer(containerName);
            return container.Resolve<T>();
        }

        /// <summary>
        /// 从默认容器中，得到对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="checkContainer"></param>
        /// <returns></returns>
        public static T Resolve<T>() where T : class
        {
            return Resolve<T>("default");
        }

        #endregion

        public static bool IsSingleton<T>(string containerName) where T : class
        {
            IUnityContainer container = GetContainer(containerName);
            return container.IsSingleton<T>();
        }

        public static bool IsSingleton<T>() where T : class
        {
            return IsSingleton<T>("default");
        }

        private static IUnityContainer GetContainer(string containerName)
        {
            return UnityConfiguration.Global.GetContainer(containerName);
        }

        private static IUnityContainer GetOrCreateContainer(string containerName)
        {
            return UnityConfiguration.Global.GetOrCreateContainer(containerName);
        }
    }
}
