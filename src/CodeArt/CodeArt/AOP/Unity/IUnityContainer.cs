using System;
using CodeArt.AppSetting;

namespace CodeArt.AOP
{
    public interface IUnityContainer
    {
        /// <summary>
        /// 容器名称
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 得到对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Resolve<T>() where T : class;


        bool IsSingleton<T>() where T : class;


        void Inject<T>(ContractImplement implement) where T : class;

    }
}
