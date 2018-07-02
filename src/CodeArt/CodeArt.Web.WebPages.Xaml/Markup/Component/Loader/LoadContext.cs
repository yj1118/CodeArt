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
using CodeArt.AppSetting;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    public sealed class LoadContext
    {
        private IComponentConnector _connector;

        private Stack<LoadingObject> _objects = new Stack<LoadingObject>();

        /// <summary>
        /// 当前正在加载的对象
        /// </summary>
        public object Target
        {
            get
            {
                return _objects.Count > 0 ? _objects.First().Target : null;
            }
        }

        public object Parent
        {
            get
            {
                return _objects.Count > 1 ? _objects.ElementAt(1).Target : null;
            }
        }

        /// <summary>
        /// 正在加载的属性名称
        /// </summary>
        public string PropertyName
        {
            get
            {
                return _objects.Count > 0 ? _objects.First().PropertyName : null;
            }
        }

        /// <summary>
        /// 正在加载的属性
        /// </summary>
        public DependencyProperty Property
        {
            get
            {
                var target = this.Target;
                var propertyName = this.PropertyName;
                if (target == null || propertyName == null) return null;
                return DependencyProperty.GetProperty(target.GetType(), propertyName);
            }
        }

        /// <summary>
        /// 正在加载的对象，所处于的模板（如果有的话）
        /// </summary>
        public FrameworkTemplate Template
        {
            get
            {
                var template = _templates.Count > 0 ? _templates.First() : null;
                if(template != null)
                {
                    var target = this.Target;
                    if(object.Equals(template,target))
                    {
                        //如果当前加载的对象本身是一个模板，那么他所处的模板在上一级
                        return _templates.Count > 1 ? _templates.ToArray()[1] : null;
                    }
                }
                return template;
            }
        }

        private LoadingObject CurrentObject
        {
            get
            {
                return _objects.Count > 0 ? _objects.First() : null;
            }
        }


        /// <summary>
        /// 追加一个正在处理的对象
        /// </summary>
        /// <param name="target"></param>
        public void PushObject(object target)
        {
            _objects.Push(new LoadingObject(target));
            TryPushTemplate(target);
        }

        /// <summary>
        /// 移除当前正在处理的对象
        /// </summary>
        public void PopObject()
        {
            var obj = _objects.Pop();

            //当组件弹出加载上下文，这意味着组件已加载完毕，触发初始化完毕的事件
            var dep = obj.Target as DependencyObject;
            if (dep != null && !dep.IsInited)
            {
                //仅初始化一次
                dep.IsInited = true;
                dep.OnInit();
            }

            TryPopTemplate(obj.Target);
        }


        private Stack<FrameworkTemplate> _templates = new Stack<FrameworkTemplate>();

        /// <summary>
        /// 尝试将对象作为模板压入栈
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool TryPushTemplate(object target)
        {
            var template = target as FrameworkTemplate;
            if (template != null)
            {
                _templates.Push(template);
                return true;
            }
            return false;
        }

        private bool TryPopTemplate(object target)
        {
            var template = target as FrameworkTemplate;
            if (template != null)
            {
                var obj = _templates.Pop();
                if (!object.Equals(target, obj)) throw new XamlException("TryPopTemplate异常，xaml引擎内部错误");
                return true;
            }
            return false;
        }

        public void PushProperty(string propertyName)
        {
            this.CurrentObject.Push(propertyName);
        }

        public void PopProperty()
        {
            this.CurrentObject.Pop();
        }


        public LoadContext(IComponentConnector connector)
        {
            _connector = connector;
        }

        private void Clear()
        {
            _connector = null;
            _objects.Clear();
        }


        #region 基于当前应用程序回话的数据上下文

        /// <summary>
        /// 获取连接器，会遍历整个加载上下文，直到得到最近的那个连接器
        /// </summary>
        public static IComponentConnector Connector
        {
            get
            {
                IComponentConnector connector = null;
                var ctxs = Contexts;
                foreach (var ctx in ctxs)
                {
                    connector = ctx._connector;
                    if (connector != null) return connector;
                }
                return null;
            }
        }


        public static void Push(LoadContext ctx)
        {
            Contexts.Push(ctx);
        }

        public static void Pop()
        {
            Contexts.Pop();
        }

        public static LoadContext Current
        {
            get
            {
                var ctxs = Contexts;
                return ctxs.Count == 0 ? null : ctxs.First();
            }
        }



        /// <summary>
        /// 正处于初始化阶段
        /// </summary>
        /// <returns></returns>
        public static bool IsInitializing
        {
            get
            {
                return (AppSession.ContainsItem(_sessionKey) && Contexts.Count > 0) 
                    || !RenderContext.IsRendering; //不是渲染阶段就是初始化阶段，这主要是考虑到组件在构造的时候的情况
            }
        }



        private const string _sessionKey = "__XamlLoadContext.Contexts";


        private static Stack<LoadContext> Contexts
        {
            get
            {
                var contexts = AppSession.GetOrAddItem<Stack<LoadContext>>(
                    _sessionKey,
                    () =>
                    {
                        return new Stack<LoadContext>();
                    });
                if (contexts == null) throw new InvalidOperationException("__XamlLoadContext.Contexts 为null,无法使用加载上下文");
                return contexts;
            }
        }

        #endregion


    }
}
