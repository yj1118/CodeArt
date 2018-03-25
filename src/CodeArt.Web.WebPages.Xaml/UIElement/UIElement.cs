using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;

using CodeArt.DTO;
using CodeArt.Runtime;
using CodeArt.Concurrent;

using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Script;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// UIElement 是作为页面界面一部分的具有可视外观并可以处理基本输入的大多数页面 UI 对象的基类。
    /// </summary>
    public abstract class UIElement : DependencyObject, IUIElement, IHtmlElementCore, ITemplateCell
    {
        #region 依赖属性

        public static DependencyProperty IdProperty { get; private set; }

        public static DependencyProperty NameProperty { get; private set; }
        public static DependencyProperty ParentProperty { get; private set; }

        public static DependencyProperty AttributesProperty { get; private set; }

        public static DependencyProperty VisibilityProperty { get; private set; }

        public static DependencyProperty ClassProperty { get; private set; }

        public static DependencyProperty StyleProperty { get; private set; }

        /// <summary>
        /// 表示codeart.js客户端框架中元素需要的代理代码
        /// </summary>
        public static DependencyProperty ProxyCodeProperty { get; private set; }

        static UIElement()
        {
            var idMetadata = new PropertyMetadata(() => { return null; });
            IdProperty = DependencyProperty.Register<string, UIElement>("Id", idMetadata);

            var nameMetadata = new PropertyMetadata(() => { return string.Empty; });
            NameProperty = DependencyProperty.Register<string, UIElement>("Name", nameMetadata);

            var parentMetadata = new PropertyMetadata(() => { return null; });
            ParentProperty = DependencyProperty.Register<DependencyObject, UIElement>("Parent", parentMetadata);

            var attributesMetadata = new PropertyMetadata(() => { return new CustomAttributeCollection(); });
            AttributesProperty = DependencyProperty.Register<CustomAttributeCollection, UIElement>("Attributes", attributesMetadata);

            var visibilityMetadata = new PropertyMetadata(() => { return Visibility.Visible; });
            VisibilityProperty = DependencyProperty.Register<Visibility, UIElement>("Visibility", visibilityMetadata);

            var classMetadata = new PropertyMetadata(() => { return string.Empty; }, OnGotClass);
            ClassProperty = DependencyProperty.Register<string, UIElement>("Class", classMetadata);

            var styleMetadata = new PropertyMetadata(() => { return string.Empty; });
            StyleProperty = DependencyProperty.Register<string, UIElement>("Style", styleMetadata);

            var proxyCodeMetadata = new PropertyMetadata(() => { return string.Empty; }, OnGotProxyCode);
            ProxyCodeProperty = DependencyProperty.Register<string, UIElement>("ProxyCode", proxyCodeMetadata);
        }

        private static void OnGotClass(DependencyObject obj, ref object baseValue)
        {
            (obj as UIElement).OnGotClass(ref baseValue);
        }

        private static void OnGotProxyCode(DependencyObject obj, ref object baseValue)
        {
            (obj as UIElement).OnGotProxyCode(ref baseValue);
        }

        public string Id
        {
            get
            {
                return GetValue(IdProperty) as string;
            }
            set
            {
                SetValue(IdProperty, value);
            }
        }

        public string Name
        {
            get
            {
                return GetValue(NameProperty) as string;
            }
            set
            {
                SetValue(NameProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public DependencyObject Parent
        {
            get
            {
                return GetValue(ParentProperty) as DependencyObject;
            }
            set
            {
                SetValue(ParentProperty, value);
            }
        }

        /// <summary>
        /// 找出第一个匹配类型的父对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FindParent<T>() where T : class
        {
            var p = this.Parent;
            while (p != null)
            {
                var t = p as T;
                if (t != null) return t;
                var e = p as UIElement;
                if (e == null) return null;
                p = e.Parent;
            }
            return null;
        }

        public virtual DependencyObject GetChild(string childName)
        {
            var connector = this as IComponentConnector;
            return connector != null ? connector.Find(childName) : null;
        }

        /// <summary>
        /// 元素的呈现状态
        /// </summary>
        public Visibility Visibility
        {
            get
            {
                return (Visibility)GetValue(VisibilityProperty);
            }
            set
            {
                SetValue(VisibilityProperty, value);
            }
        }

        /// <summary>
        /// html中的class
        /// </summary>
        public string Class
        {
            get
            {
                return GetValue(ClassProperty) as string;
            }
            set
            {
                SetValue(ClassProperty, value);
            }
        }

        protected virtual void OnGotClass(ref object baseValue) { }

        public string Style
        {
            get
            {
                return GetValue(StyleProperty) as string;
            }
            set
            {
                SetValue(StyleProperty, value);
            }
        }

        /// <summary>
        /// 表示codear.js框架中的代理代码
        /// </summary>
        public string ProxyCode
        {
            get
            {
                return GetValue(ProxyCodeProperty) as string;
            }
            set
            {
                SetValue(ProxyCodeProperty, value);
            }
        }

        protected virtual void OnGotProxyCode(ref object baseValue) { }

        #endregion

        #region 行为

        #region 自定义脚本行为

        private Dictionary<string, Func<ScriptView, IScriptView>> _registeredScriptActions;

        /// <summary>
        /// 注册一个脚本行为
        /// 每一个UI元素都可以具备一系列的脚本行为，至于如何使用这些脚本行为有元素本身的设计决定；
        /// 该方法请在初始化阶段执行（构造函数、Init事件中注册），不能用于渲染阶段
        /// 注册脚本行为是多线程共享的
        /// </summary>
        public void RegisterScriptAction(string actionName, Func<ScriptView, IScriptView> action,Action<ViewEventProcessor> setEventProcessor)
        {
            if (_registeredScriptActions == null) _registeredScriptActions = new Dictionary<string, Func<ScriptView, IScriptView>>();
            if (_registeredScriptActions.ContainsKey(actionName)) throw new XamlException("重复注册脚本行为" + actionName);
            _registeredScriptActions.Add(actionName, action);

            if (setEventProcessor != null)
            {
                ViewEventProcessor processor = new ViewEventProcessor();
                setEventProcessor(processor);
                this.ScriptCallback += (view) =>
                {
                    view.RegisterViewEventProcessor(actionName, processor);
                    return view;
                };
            }
        }

        /// <summary>
        /// 注册一个脚本行为
        /// 每一个UI元素都可以具备一系列的脚本行为，至于如何使用这些脚本行为由元素本身的设计决定；
        /// 该方法请在初始化阶段执行（构造函数、Init事件中注册），不能用于渲染阶段
        /// 注册脚本行为是多线程共享的
        /// </summary>
        public void RegisterScriptAction(string actionName, Func<ScriptView, IScriptView> action)
        {
            RegisterScriptAction(actionName, action, null);
        }

        /// <summary>
        /// 执行注册的脚本行为
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="arg"></param>
        /// <returns></returns>
        internal bool CallRegisteredScriptAction(string actionName, ScriptView arg, ref IScriptView returnValue)
        {
            if(_registeredScriptActions != null)
            {
                Func<ScriptView, IScriptView> action = null;
                if(_registeredScriptActions.TryGetValue(actionName,out action))
                {
                    returnValue = action(arg);
                    return true;
                }
            }
            return false;
        }

        #endregion


        /// <summary>
        /// 执行组件的脚本行为
        /// </summary>
        /// <param name="componentName">当componentName为null，那么执行本对象的方法</param>
        /// <param name="action"></param>
        /// <param name="arg"></param>
        internal IScriptView CallScriptAction(string componentName, string actionName, ScriptView arg)
        {
            object result = null;
            if (string.IsNullOrEmpty(componentName) || string.Equals(this.Name, componentName))
            {
                result = ExecuteAction(this, actionName, arg);
            }
            else
            {
                var e = GetChild(componentName) as UIElement;
                if (e == null) throw new XamlException("没有找到名称为" + componentName + "的" + typeof(UIElement).FullName + "组件");
                result = ExecuteAction(e, actionName, arg) ?? ExecuteAction(this, actionName, arg); //先在对应的元素上找方法执行，如果找不到，就在该组件上找
            }
            var view = result as IScriptView;
            if (view == null) throw new XamlException(string.Format(Strings.CallServerMethodError, actionName, typeof(IScriptView).Name));
            return view;
        }

        private readonly static MethodParameter[] ActionParameters = new MethodParameter[] { MethodParameter.Create<ScriptView>() };

        private static object ExecuteAction(UIElement e, string actionName, ScriptView arg)
        {
            IScriptView result = null;
            if (e.CallRegisteredScriptAction(actionName, arg, ref result)) return result; //优先执行注册的行为
            var action = e.ObjectType.ResolveMethod(actionName, ActionParameters);
            if(action != null)
            {
                using (var temp = ArgsPool.Borrow1())
                {
                    var args = temp.Item;
                    args[0] = arg;
                    return action.Invoke(e, args);
                }  
            }

            return null;

            //IScriptView result = null;
            //if (e.CallRegisteredScriptAction(actionName, arg, ref result)) return result; //优先执行注册的行为
            //var action = DependencyAction.GetAction(e.ObjectType, actionName);
            //if (action != null)
            //{
            //    if (!action.AllowClientAccess) throw new XamlException("组件方法" + actionName + "未公开");
            //    return e.CallAction(action, arg);
            //}

            ////如果未能成功在e上执行行文，那么找父亲对象的方法
            //var parent = e.Parent as UIElement;
            //if (parent != null) return parent.CallScriptAction(null, actionName, arg);

            //throw new XamlException("未能执行" + actionName);
        }


        #endregion

        //#region 覆盖自身行为

        /////// <summary>
        /////// 重写脚本行为
        /////// </summary>
        /////// <param name="action"></param>
        /////// <param name="procedure"></param>
        ////public void OverrideScriptAction(DependencyAction action, Func<ScriptView, object> procedure)
        ////{
        ////    OverrideAction<ScriptView>(action, procedure);
        ////}

        //public void OverrideAction(DependencyAction action, Func<object> procedure)
        //{
        //    OverrideAction(action, (args) =>
        //    {
        //        return ActionEngine.Execute(args, procedure);
        //    });
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T0"></typeparam>
        ///// <param name="action"></param>
        ///// <param name="procedure"></param>
        //public void OverrideAction<T0>(DependencyAction action, Func<T0, object> procedure)
        //{
        //    OverrideAction(action, (args) =>
        //    {
        //        return ActionEngine.Execute(args, procedure);
        //    });
        //}

        ///// <summary>
        ///// 覆盖自身行为
        ///// </summary>
        ///// <typeparam name="T0"></typeparam>
        ///// <typeparam name="T1"></typeparam>
        ///// <param name="action"></param>
        ///// <param name="procedure"></param>
        //public void OverrideAction<T0, T1>(DependencyAction action, Func<T0, T1, object> procedure)
        //{
        //    OverrideAction(action, (args) =>
        //    {
        //        return ActionEngine.Execute(args, procedure);
        //    });
        //}

        ///// <summary>
        ///// 覆盖自身行为
        ///// </summary>
        ///// <typeparam name="T0"></typeparam>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="T2"></typeparam>
        ///// <param name="action"></param>
        ///// <param name="procedure"></param>
        //public void OverrideAction<T0, T1, T2>(DependencyAction action, Func<T0, T1, T2, object> procedure)
        //{
        //    OverrideAction(action, (args) =>
        //    {
        //        return ActionEngine.Execute(args, procedure);
        //    });
        //}

        ///// <summary>
        ///// 覆盖自身行为
        ///// </summary>
        ///// <typeparam name="T0"></typeparam>
        ///// <typeparam name="T1"></typeparam>
        ///// <typeparam name="T2"></typeparam>
        ///// <typeparam name="T3"></typeparam>
        ///// <param name="action"></param>
        ///// <param name="procedure"></param>
        //public void OverrideAction<T0, T1, T2, T3>(DependencyAction action, Func<T0, T1, T2, T3, object> procedure)
        //{
        //    OverrideAction(action, (args) =>
        //    {
        //        return ActionEngine.Execute(args, procedure);
        //    });
        //}

        //private void OverrideAction(DependencyAction action, Func<object[], object> execute)
        //{
        //    this.ActionPreCall += (sender, e) =>
        //    {
        //        if (e.Action == action)
        //        {
        //            e.Allow = false;
        //            e.ReturnValue = execute(e.Arguments);
        //        }
        //    };
        //}

        //#endregion


        /// <summary>
        /// 自定义属性
        /// </summary>
        public CustomAttributeCollection Attributes
        {
            get
            {
                return GetValue(AttributesProperty) as CustomAttributeCollection;
            }
            set
            {
                SetValue(AttributesProperty, value);
            }
        }


        /// <summary>
        /// 自定义属性是否为表达式的定义
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsExpression(string name)
        {
            var attr = this.Attributes.GetAttribute(name);
            return attr != null && (attr.IsExpression(CustomAttribute.ValueProperty));
        }

        public bool ProxyCodeIsExpression()
        {
            return this.IsExpression(ProxyCodeProperty) || this.IsExpression("data-proxy");
        }

        public override void OnInit()
        {
            //为了节省内存，固化attributes，这样其余线程访问时，会加载该对象作为本地值，而不是重新new新的对象
            if (this.Attributes == null) this.Attributes = new CustomAttributeCollection();
        }

        public event RoutedEventHandler PreRender;

        protected virtual void OnPreRender()
        {
            EventHandlerAttribute.Process(this, RoutedEvent.PreRender);
            if (this.PreRender != null)
            {
                this.PreRender(this, this);
            }
        }

        public void Render(PageBrush brush)
        {
            RenderContext.Current.PushObject(this);
            OnPreRender();
            if (this.Visibility == Visibility.Collapsed) return; //如果隐藏元素，则不绘制
            Draw(brush);
            DrawScriptCallback(brush);
            RenderContext.Current.PopObject();
        }

        protected abstract void Draw(PageBrush brush);

        /// <summary>
        /// 脚本回调事件，当整个页面加载完后，执行该脚本内容
        /// </summary>
        public event ScriptActionEventHandler ScriptCallback;

        /// <summary>
        /// 绘制脚本回调方法指令
        /// </summary>
        private void DrawScriptCallback(PageBrush brush)
        {
            if (this.ScriptCallback != null)
            {
                var view = new ScriptView();
                var result = this.ScriptCallback(view);
                brush.DrawScriptCallback(result);
            }
        }

        private FrameworkTemplate _belongTemplate;

        /// <summary>
        /// 对象所属的模板，该属性不是依赖方法，也不允许外界赋值
        /// Parent属性的改变，不会引起BelongTemplate的改变，在加载阶段已经固定了组件所属模板
        /// </summary>
        public FrameworkTemplate BelongTemplate
        {
            get
            {
                if(_belongTemplate == null)
                {
                    //模板的概念由整个xaml空间共享，而不是control类独有的，UIElement和Control在同一命名空间下，内聚高，所以可以转换
                    var parent = this.Parent as Control;
                    while(parent != null)
                    {
                        var template = parent.Template;
                        if (template != null) return template;
                        parent = parent.Parent as Control;
                    }
                    return null;
                }
                return _belongTemplate;
            }
            internal set
            {
                _belongTemplate = value;
            }
        }

        ///// <summary>
        ///// 获取元素所处的模板对象，如果元素不在模板中，那么返回null
        ///// </summary>
        ///// <returns></returns>
        //public FrameworkTemplate GetBelongTemplate()
        //{
        //    var parent = this.Parent;
        //    while (parent != null)
        //    {
        //        var template = parent as FrameworkTemplate;
        //        if (template != null) return template;
        //        var uiParent = parent as UIElement;
        //        if (uiParent == null) return null;
        //        parent = uiParent.Parent;
        //    }
        //    return null;
        //}

        /// <summary>
        /// 获取当前以模板方式应用本组件的对象
        /// </summary>
        /// <returns></returns>
        public object GetTemplateParent()
        {
            var template = this.BelongTemplate;
            return template != null ? template.TemplateParent : null;
        }

        #region 页面相关

        //public WebPage Page
        //{
        //    get
        //    {
        //        return WebPageContext.Current.Page;
        //    }
        //}

        /// <summary>
        /// 是否为脚本回调环境
        /// </summary>
        public bool IsScriptCallback
        {
            get
            {
                return !WebPageContext.Current.IsGET;
            }
        }

        public T GetQueryValue<T>(string name,T defaultValue)
        {
            return WebPageContext.Current.GetQueryValue<T>(name, defaultValue);
        }

        #endregion

        internal override bool SetValue(string propertyName, object propertyValue, bool ignoreExpression)
        {
            if (!base.SetValue(propertyName, propertyValue, ignoreExpression))
            {
                this.Attributes.SetValue(this, propertyName, propertyValue, ignoreExpression);
            }
            return true;
        }

        public object GetValue(string propertyName)
        {
            return GetValue(propertyName, false);
        }

        internal object GetValue(string propertyName, bool ignoreExpression)
        {
            object value = null;
            if (!base.GetValue(propertyName, ref value, ignoreExpression))
            {
                value = this.Attributes.GetValue(propertyName, ignoreExpression);
            }
            return value;
        }

    }
}
