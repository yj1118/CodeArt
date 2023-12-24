using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 组件加载器工厂
    /// </summary>
    internal static class ComponentLoaderFactory
    {
        private static Func<Type, IComponentLoaderFactory> _getLoaderFactory = LazyIndexer.Init<Type, IComponentLoaderFactory>((objType) =>
        {
            var factoryAttr = ComponentLoaderFactoryAttribute.GetAttribute(objType);
            if (factoryAttr != null)
            {
                var factoryType = factoryAttr.LoaderFactoryType;
                if (factoryType != null) return SafeAccessAttribute.CreateSingleton<IComponentLoaderFactory>(factoryType);
            }
            return ManualComponentLoader.Instance;
        });

        public static ComponentLoader Create(object obj, HtmlNode objNode)
        {
            var d = obj as DependencyObject;
            var type = d != null ? d.ObjectType : obj.GetType();

            var factory = _getLoaderFactory(type);
            return factory.CreateLoader(obj, objNode);
        }
    }
}
