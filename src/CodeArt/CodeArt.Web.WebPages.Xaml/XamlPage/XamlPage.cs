using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;
using CodeArt.Web.WebPages;
using CodeArt.Util;
using CodeArt.Concurrent;
using CodeArt.DTO;

using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Web.WebPages.Xaml.Script;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Runtime;

namespace CodeArt.Web.WebPages.Xaml
{
    [SafeAccess]
    public sealed class XamlPage : WebText
    {
        private XamlPage() { }

        #region 页面代理

        private bool ProcessProxy()
        {
            var proxy = GetComponent(this.VirtualPath) as PageProxy;
            if (proxy != null)
            {
                proxy.LoadPinned();

                XamlUtil.UsingRender((brush) =>
                {
                    proxy.OnLoad(); //OnLoad的触发，必须在呈现环境初始化之后进行
                    proxy.Process(this.PageContext);
                });
                return true;
            }
            return false;
        }


        protected override void ProcessGET()
        {
            if (ProcessProxy()) return;
            base.ProcessGET();
        }

        protected override void ProcessPOST()
        {
            if (ProcessProxy()) return;
            base.ProcessPOST();
        }

        #endregion


        protected override string GetTextContent(string error)
        {
            return GetPageCode(this.VirtualPath, error);
        }

        #region 页面刷

        public static string GetPageCode(string virtualPath, string errorInfo = null)
        {
            var component = GetComponent(virtualPath);
            var ui = component as UIElement;
            return ui == null ? GetRawCode(virtualPath) : GetPageCode(ui, errorInfo);
        }

        private static string GetRawCode(string virtualPath)
        {
            return PageUtil.GetRawCode(virtualPath);
        }

        /// <summary>
        /// 获得页面组件
        /// </summary>
        /// <returns></returns>
        private static object GetComponent(string virtualPath)
        {
            return XPCFactory.Create(virtualPath);
        }


        private static string GetPageCode(UIElement element, string error)
        {
            //在获取页面代码之前，先加载固化值
            element.LoadPinned();

            if (!string.IsNullOrEmpty(error))
            {
                SetError(element, error);
            }

            var code = string.Empty;
            XamlUtil.UsingRender((brush) =>
            {
                element.OnLoad(); //OnLoad的触发，必须在呈现环境初始化之后进行

                AppendLanguage(brush);
                element.Render(brush);
                code = brush.GetCode(false); //输出完整代码，不论body是否存在
            });
            return code;
        }

        private static void AppendLanguage(PageBrush brush)
        {
            AssetStrings.Global.Render(brush); //添加多语言支持
            AssetStrings.Current.Render(brush);
        }

        private static void SetError(UIElement ui, string error)
        {
            //错误页内容
            if (ui != null)
            {
                var e = ui.GetChild("display") as Run;
                if (e == null) throw new XamlException("由于定义了error,但是没有定义名称为display的Run类型组件");
                e.Content = error;
            }
        }


        #endregion

        #region 事件

        protected override string CallWebMethod(DTObject args)
        {
            //在处理事件之前，先加载固化值
            UIElement component = GetComponent(this.VirtualPath) as UIElement;
            if (component == null) throw new XamlException("无法处理事件，组件不是" + typeof(UIElement).FullName);
            component.LoadPinned();

            string result = null;
            XamlUtil.UsingRender((brush) =>
            {
                component.OnLoad();
                result = CallWebMethod(component, args);
            });
            return result;
        }

        private static string CallWebMethod(UIElement component, DTObject args)
        {
            if (args == null) throw new XamlException("处理事件参数错误");

            //{component,action,argument:{sender:{id,metadata:{}},elements:[{id,metadata:{}}]}}
            var actionName = args.GetValue<string>("action", string.Empty);
            if (string.IsNullOrEmpty(actionName)) throw new XamlException("处理事件参数错误，没有指定action");

            var componentName = args.GetValue<string>("component", string.Empty);

            DTObject argument = null;
            if (!args.TryGetObject("argument", out argument)) throw new XamlException("处理事件参数错误，没有指定argument");

            var view = component.CallScriptAction(componentName, actionName, new ScriptView(argument)) as IScriptView;
            return view.GetDataCode();
        }

        

        #endregion

        public static readonly XamlPage Instance = new XamlPage();

    }

}
