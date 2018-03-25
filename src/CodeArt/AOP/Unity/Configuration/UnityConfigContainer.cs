using System;

using CodeArt.AppSetting;

namespace CodeArt.AOP
{
    /// <summary>
    /// 通过配置得到的容器
    /// </summary>
    internal class UnityConfigContainer : IUnityContainer
    {
        private string _name;
        /// <summary>
        /// 容器名称
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private InterfaceMapper _mapper;
        public InterfaceMapper Mapper
        {
            get { return _mapper; }
        }

        internal UnityConfigContainer(string name, InterfaceMapper mapper)
        {
            this._name = name;
            _mapper = mapper;
        }

        public T Resolve<T>() where T : class
        {
            return _mapper.GetInstance<T>();
        }

        public bool IsSingleton<T>() where T : class
        {
            var info = _mapper.GetImplementInfo<T>();
            if (info == null) return false;
            return info.IsSingleton;
        }

        public void Inject<T>(ContractImplement implement) where T : class
        {
            _mapper.AddImplement(typeof(T), new InterfaceImplement()
            {
                ImplementType = implement.ImplementType,
                IsSingleton = implement.IsSingleton
            });
        }


        public static readonly IUnityContainer Empty = new UnityContainerEmpty();

        private class UnityContainerEmpty : IUnityContainer
        {
            /// <summary>
            /// 容器名称
            /// </summary>
            public string Name
            {
                get { return string.Empty; }
                set { }
            }

            /// <summary>
            /// 得到对象实例
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public T Resolve<T>() where T : class
            {
                return null;
            }

            public bool IsSingleton<T>() where T : class
            {
                return false;
            }

            public void Inject<T>(ContractImplement implement) where T : class
            {

            }

            public UnityContainerEmpty() { }
        }

    }
}
