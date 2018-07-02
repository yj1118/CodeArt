using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.DTO;


namespace CodeArt.ServiceModel.Mock
{
    [SafeAccess]
    public abstract class ContractEventFactory : IContractEventFactory
    {
        public IContractEvent Create(DTObject dto)
        {
            var typeName = dto.GetValue<string>("type", string.Empty);
            if (string.IsNullOrEmpty(typeName)) throw new ServiceException("没有定义IContractEvent的类型，无法实例化ContractEvent");
            var type = _getEventType(typeName);
            if (type == null) throw new ServiceException("没有找到" + typeName + "的类型，无法实例化ContractEvent");

            var dtoArgs = dto.GetObject("args", DTObject.Empty);
            var args = GetArgs(dtoArgs);
            var ce = Activator.CreateInstance(type, args) as IContractEvent;
            if (ce == null) throw new TypeMismatchException(type, typeof(IContractEvent));
            return ce;
        }

        /// <summary>
        /// 根据 args 的dto定义，得到实际.net类型的args
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract object[] GetArgs(DTObject args);

        #region 静态成员

        internal static IContractEvent CreateCE(DTObject dto)
        {
            var typeName = dto.GetValue<string>("type", string.Empty);
            if (string.IsNullOrEmpty(typeName)) throw new ServiceException("没有定义IContractEvent的类型，无法实例化ContractEvent");
            var factoryType = _getFactoryType(typeName);
            if (factoryType == null) throw new ServiceException("没有找到" + typeName + "的类型创建工厂，无法实例化ContractEvent");

            var factory = SafeAccessAttribute.CreateInstance<IContractEventFactory>(factoryType);
            return factory.Create(dto);
        }

        private static Func<string, Type> _getFactoryType = LazyIndexer.Init<string, Type>((typeName) =>
        {
            foreach (var assemblyName in _assemblyNames)
            {
                string fullTypeName = string.Format("{0}Factory,{1}", typeName, assemblyName);
                Type factoryType = Type.GetType(fullTypeName);
                if (factoryType != null) return factoryType;
            }
            return null;
        });

        private static Func<string, Type> _getEventType = LazyIndexer.Init<string, Type>((typeName) =>
        {
            foreach (var assemblyName in _assemblyNames)
            {
                string fullTypeName = string.Format("{0},{1}", typeName, assemblyName);
                Type eventType = Type.GetType(fullTypeName);
                if (eventType != null) return eventType;
            }
            return null;
        });

        private static List<string> _assemblyNames = new List<string>();

        static ContractEventFactory()
        {
            _assemblyNames.AddRange(ServiceConfiguration.Current.ContractEventsAssemblyNames);
            var assembly = Assembly.GetCallingAssembly();
            if (assembly != null) _assemblyNames.Add(assembly.GetName().Name);

            assembly = Assembly.GetEntryAssembly();
            if (assembly != null) _assemblyNames.Add(assembly.GetName().Name);
        }


        #endregion


    }
}
