using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;

using HtmlAgilityPack;
using CodeArt.Runtime;
using CodeArt.HtmlWrapper;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    public sealed class XamlReader : IXamReader
    {
        private XamlReader() { }

        public object Read(string xaml)
        {
            if (string.IsNullOrEmpty(xaml)) return null;
            var node = XamlUtil.GetNode(xaml);
            if (node == null) return null;
            XmlnsDictionary.Collect(node);

            //创建元素
            var obj = ComponentFactory.Create(node.OriginalName);
            var type = obj as Type;
            if (type != null)
            {
                //是基础类型
                return DataUtil.ToValue(node.InnerHtml, type);
            }
            else
            {
                Load(obj, node);
            }
            return obj;
        }

        /// <summary>
        /// 加载xaml文件中的信息到obj中
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="xaml"></param>
        /// <param name="connector"></param>
        public void Load(object obj, string xaml)
        {
            if (obj == null) return;
            var node = XamlUtil.GetNode(xaml);
            if (node == null) return;
            XmlnsDictionary.Collect(node);
            Load(obj, node);
        }

        private void Load(object obj, HtmlNode objNode)
        {
            //设置加载上下文
            var connector = obj as IComponentConnector;
            var ctx = new LoadContext(connector);
            LoadContext.Push(ctx);

            try
            {
                //加载元素信息
                var loader = ComponentLoaderFactory.Create(obj, objNode);
                loader.LoadComponent(obj, objNode);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                LoadContext.Pop();
            }
        }


        #region 静态版本


        /// <summary>
        /// 加载xaml文件，并与组件连接器连接
        /// </summary>
        /// <param name="component"></param>
        /// <param name="xaml"></param>
        public static object ReadComponent(string xaml)
        {
            return XamlReader.Instance.Read(xaml);
        }

        ///// <summary>
        ///// 从程序集资源中的xaml文件加载组件
        ///// </summary>
        ///// <param name="assemblyName"></param>
        ///// <param name="resourceName"></param>
        ///// <returns></returns>
        //public static object ReadComponent(string assemblyName, string resourceName)
        //{
        //    var xaml = AssemblyResource.LoadText(assemblyName, resourceName);
        //    if (string.IsNullOrEmpty(xaml)) return null;
        //    return ReadComponent(xaml);
        //}

        public static void LoadComponent(object obj, string xaml)
        {
            XamlReader.Instance.Load(obj, xaml);
        }

        //public static void LoadComponent(object obj, string assemblyName, string resourceName)
        //{
        //    var xaml = AssemblyResource.LoadText(assemblyName, resourceName);
        //    LoadComponent(obj, xaml);
        //}


        private readonly static XamlReader Instance = new XamlReader();

        #endregion

    }

}
