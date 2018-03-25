using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.IO;
using System.Xml;


using CodeArt.AppSetting;

namespace CodeArt.AOP
{
    public class UnityConfiguration : IConfigurationSectionHandler
    {
        private Dictionary<string, IUnityContainer> _containers = new Dictionary<string, IUnityContainer>();

        internal IUnityContainer GetOrCreateContainer(string containerName)
        {
            var container = GetContainer(containerName);
            if(container == null)
            {
                lock(_containers)
                {
                    container = GetContainer(containerName);
                    if(container == null)
                    {
                        container = new UnityConfigContainer(containerName, new InterfaceMapper());
                        _containers.Add(containerName, container);
                    }
                }
            }
            return container;
        }

        internal IUnityContainer GetContainer(string containerName)
        {
            IUnityContainer container = null;
            _containers.TryGetValue(containerName, out container);
            return container;
        }

        internal UnityConfiguration()
        {
        }

        public object Create(object parent, object configContext, XmlNode section)
        {
            foreach (XmlNode node in section.ChildNodes)
            {
                switch (node.Name)
                {
                    case "containers":
                        {
                            foreach (XmlNode item in node.ChildNodes)
                            {
                                if (item.Name == "container")
                                {
                                    UnityConfigContainer container = GetContainerFromXmlNode(item);
                                    _containers.Add(container.Name, container);
                                }
                            }
                        }
                        break;
                }
            }
            return this;
        }

        private UnityConfigContainer GetContainerFromXmlNode(XmlNode node)
        {
            //UnityContainerTypeMapper mapper = new UnityContainerTypeMapper();
            InterfaceMapper mapper = new InterfaceMapper();
            string containerName = node.Attributes["name"] == null ? "default" : node.Attributes["name"].Value;
            //string baseContainerName = node.Attributes["base"] == null ? string.Empty : node.Attributes["base"].Value;
            //if (!string.IsNullOrEmpty(baseContainerName))
            //{
            //    UnityConfigContainer baseContainer = this.GetContainer(baseContainerName) as UnityConfigContainer;
            //    if (baseContainer == null)
            //        throw new UnityException("没有找到名称为" + baseContainerName + "的反转控制容器");
            //    baseContainer.Mapper.FillMapper(mapper);//从基础容器中填充信息
            //}

            foreach (XmlNode item in node.ChildNodes)
            {
                switch (item.Name)
                {
                    case "types":
                        {
                            foreach (XmlNode typeNode in item.ChildNodes)
                            {
                                if (typeNode.Name == "add")
                                {
                                    string interfaceName = typeNode.Attributes["type"].Value, implementName = typeNode.Attributes["mapTo"].Value;
                                    bool isSingleton = typeNode.Attributes["isSingleton"] != null && typeNode.Attributes["isSingleton"].Value == "true";
                                    UnityResolveArgument[] args = null;
                                    XmlNodeList list =  typeNode.SelectNodes("params/add");
                                    if (list.Count > 0)
                                    {
                                        var argsCount = list.Count;
                                        args = new UnityResolveArgument[argsCount];
                                        for (var i = 0; i < argsCount; i++)
                                        {
                                            XmlNode argNode = list[i];
                                            args[i] = new UnityResolveArgument(argNode.Attributes["name"].Value, argNode.Attributes["value"].Value);
                                        }
                                    }

                                    Type impType = Type.GetType(implementName);
                                    if (impType == null)
                                        throw new UnityException(string.Format("没有找到 {0} 对应的类型 {1} ", interfaceName, implementName));
  
                                    Type interfaceType = Type.GetType(interfaceName);
                                    if (interfaceType == null)
                                        throw new UnityException(string.Format("没有找到类型 {0} ", interfaceName));


                                    InterfaceImplement impInfo = new InterfaceImplement
                                    {
                                        ImplementType = impType,
                                        IsSingleton = isSingleton,
                                        InitMethod = (instance) =>
                                           {
                                               IUnityResolve resolve = instance as IUnityResolve;
                                               if (resolve != null)
                                                   resolve.Init(args);
                                           }
                                    };
                                    mapper.AddImplement(interfaceType, impInfo);
                                }
                            }
                        }
                        break;
                }
            }

            return new UnityConfigContainer(containerName, mapper);
        }

        #region 当前配置
        /// <summary>
        /// 当前站的配置信息
        /// </summary>
        internal static readonly UnityConfiguration Global;

        static UnityConfiguration()
        {
            Global = ConfigurationManager.GetSection("codeArt.unity") as UnityConfiguration;
            if (Global == null) Global = new UnityConfiguration();
        }

        #endregion
    }
}
