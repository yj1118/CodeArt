using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Runtime;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Util;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    [SafeAccess]
    public class ComponentLoader : IComponentLoader
    {
        protected ComponentLoader() { }

        public void LoadComponent(object obj, HtmlNode objNode)
        {
            var ctx = LoadContext.Current;
            ctx.PushObject(obj);

            if (ctx.Template != null)
            {
                var ui = obj as UIElement;
                if (ui != null) ui.BelongTemplate = ctx.Template;
            }

            Load(obj, objNode);

            ProcessProxyCode(obj, objNode);

            var name = objNode.GetDeclareName();

            var connector = LoadContext.Connector;
            if (connector != null && !string.IsNullOrEmpty(name))
            {
                connector.Connect(name, obj);
            }

            ctx.PopObject();
        }

        private void ProcessProxyCode(object obj, HtmlNode objNode)
        {
            var e = obj as UIElement;
            if (e != null && !e.ProxyCodeIsExpression())
            {
                e.ProxyCode = ProxyCodeCombine.CombineCode(e, objNode); //合并生成的proxy代码和用户手工指定的proxy代码
                var proxyCode = e.ProxyCode;
                if (ProxyCodeExtend.TryFillCode(e, objNode, ref proxyCode))
                    e.ProxyCode = proxyCode;
                e.Attributes.RemoveValue("data-proxy"); //移除自定义属性data-proxy，避免重复输出
            }
        }


        protected virtual void Load(object obj, HtmlNode objNode)
        {
            LoadProperties(obj, objNode);
            LoadContent(obj, objNode);
        }


        /// <summary>
        /// 加载属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objNode"></param>
        /// <param name="context"></param>
        protected virtual void LoadProperties(object obj, HtmlNode objNode)
        {
            PropertiesLoader.Load(obj, objNode);
        }

        /// <summary>
        /// 加载内容
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objNode"></param>
        /// <param name="context"></param>
        protected virtual void LoadContent(object obj, HtmlNode objNode)
        {
            ContentLoader.Load(obj, objNode);
        }

        /// <summary>
        /// 根据节点创建组件，该方法内部使用，不会新建加载上下文
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static object CreateComponent(HtmlNode node, object parent)
        {
            var obj = ComponentFactory.Create(node.OriginalName);
            var type = obj as Type;
            if (type != null)
            {
                //是基础类型
                return DataUtil.ToValue(node.InnerHtml, type);
            }
            else
            {
                var e = obj as UIElement;
                if (e != null) e.SetValue(UIElement.ParentProperty, parent);

                //加载元素信息
                var loader = ComponentLoaderFactory.Create(obj, node);
                loader.LoadComponent(obj, node);
                return obj;
            }
        }

        public static ComponentLoader Instance = new ComponentLoader();

    }
}
