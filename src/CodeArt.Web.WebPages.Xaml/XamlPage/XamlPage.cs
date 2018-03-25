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

namespace CodeArt.Web.WebPages.Xaml
{
    [SafeAccess]
    public sealed class XamlPage : WebText
    {
        private XamlPage() { }

        #region 页面代理

        private bool ProcessProxy()
        {
            var proxy = GetComponent() as PageProxy;
            if (proxy != null)
            {
                proxy.LoadPinned();
                proxy.OnLoad();

                proxy.Process(this.PageContext);
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
            var component = GetComponent();
            var ui = component as UIElement;
            return ui == null ? GetRawCode() : GetPageCode(ui, error); 
        }

        private string GetRawCode()
        {
            return PageUtil.GetRawCode(this.VirtualPath);
        }

        /// <summary>
        /// 获得页面组件
        /// </summary>
        /// <returns></returns>
        private object GetComponent()
        {
            return XPCFactory.Create(this.VirtualPath);
        }

        #region 页面刷

        private static string GetPageCode(UIElement element, string error)
        {
            //在获取页面代码之前，先加载固化值
            element.LoadPinned();
            element.OnLoad();


            if (!string.IsNullOrEmpty(error))
            {
                SetError(element, error);
            }

            var code = string.Empty;
            using (var temp = XamlUtil.BrushPool.Borrow())
            {
                var brush = temp.Item;
                using (var ctx = RenderContext.Borrow())
                {
                    RenderContext.Current = ctx.Item;
                    AppendLanguage(brush);
                    element.Render(brush);
                    RenderContext.Current = null;
                    code = brush.GetCode();
                }
            }
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

        protected override object CallWebMethod(object[] args)
        {
            var component = this.GetComponent() as UIElement;
            if (component == null) throw new XamlException("无法处理事件，组件不是" + typeof(UIElement).FullName);

            //在处理事件之前，先加载固化值
            component.LoadPinned();
            component.OnLoad();

            var arg = args.Length == 0 ? DTObject.Create() : args[0] as DTObject;
            if (arg == null) throw new XamlException("处理事件参数错误");

            //{component,action,argument:{sender:{id,metadata:{}},elements:[{id,metadata:{}}]}}
            var actionName = arg.GetValue<string>("action", string.Empty);
            if (string.IsNullOrEmpty(actionName)) throw new XamlException("处理事件参数错误，没有指定action");

            var componentName = arg.GetValue<string>("component", string.Empty);

            DTObject argument = null;
            if (!arg.TryGetObject("argument", out argument)) throw new XamlException("处理事件参数错误，没有指定argument");

            var view = component.CallScriptAction(componentName, actionName, new ScriptView(argument)) as IScriptView;
            return view.Output();
        }

        #endregion

        public static readonly XamlPage Instance = new XamlPage();

    }

}
