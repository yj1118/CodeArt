using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml
{
    internal static class TemplateFactory
    {
        /// <summary>
        /// 创建模板
        /// </summary>
        /// <typeparam name="T">模板类型</typeparam>
        /// <param name="objType"></param>
        /// <returns></returns>
        public static T Create<T>(object obj, DependencyProperty templateProperty) where T : class
        {
            //为了保证不同组件的实例各自维护自己的模板，所以不缓存模板对象的创建
            var result = CreateTemplate(obj, templateProperty.Name);
            if (result == null) return null;
            var template = result as T;
            if (template == null) throw new TypeMismatchException(result.GetType(), typeof(T));
            return template;
        }

        private static object CreateTemplate(object obj, string templatePropertyName)
        {
            var code = GetTemplateCode(obj, templatePropertyName);
            if (code == null) return null;
            XmlnsDictionary.New(); //模板内的加载，需要使用自己的xmlns空间

#if (DEBUG)
            FrameworkTemplate.PushTrackCode(code);
            object template = XamlReader.ReadComponent(code);
            FrameworkTemplate.PopTrackCode();
#endif

#if (!DEBUG)
            object template = XamlReader.ReadComponent(code);
#endif
            XmlnsDictionary.Pop();
            //if (template == null) throw new WebException("没有找到类型" + objType.FullName + "的模板" + templatePropertyName + "定义");
            if (template == null) return null;
            return template;
        }

        private static string GetTemplateCode(object obj, string templatePropertyName)
        {
            var objType = XamlUtil.GetObjectType(obj);
            var factory = _getTemplateCodeFactory(objType)(templatePropertyName);
            return factory.GetTemplateCode(obj, templatePropertyName);
        }

        private static Func<Type, Func<string, ITemplateCodeFactory>> _getTemplateCodeFactory = LazyIndexer.Init<Type, Func<string, ITemplateCodeFactory>>((objType) =>
        {
            return LazyIndexer.Init<string, ITemplateCodeFactory>((templatePropertyName) =>
            {
                var factoryType = TemplateCodeFactoryAttribute.GetAttribute(objType, templatePropertyName).TemplateCodeFactoryType;
                return SafeAccessAttribute.CreateSingleton<ITemplateCodeFactory>(factoryType);
            });
        });

    }
}