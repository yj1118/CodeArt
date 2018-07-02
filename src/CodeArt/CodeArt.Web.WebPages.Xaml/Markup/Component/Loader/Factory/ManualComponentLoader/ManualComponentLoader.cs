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
    /// 通过ComponentLoaderAttribute手工指定加载器
    /// </summary>
    [SafeAccess]
    public class ManualComponentLoader : IComponentLoaderFactory
    {
        private static Func<Type, ComponentLoader> _getLoader = LazyIndexer.Init<Type, ComponentLoader>((objType) =>
        {
            var loaderAttr = ComponentLoaderAttribute.GetAttribute(objType);
            if (loaderAttr != null)
            {
                var loaderType = loaderAttr.LoaderType;
                if (loaderType != null) return SafeAccessAttribute.CreateSingleton<ComponentLoader>(loaderType);
            }
            return ComponentLoader.Instance;
        });

        public ComponentLoader CreateLoader(object obj, HtmlNode objNode)
        {
            var type = obj.GetType();
            return _getLoader(type);
        }

        public static readonly ManualComponentLoader Instance = new ManualComponentLoader();
    }
}
