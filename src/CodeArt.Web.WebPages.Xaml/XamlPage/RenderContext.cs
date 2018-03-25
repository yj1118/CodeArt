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
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    public sealed class RenderContext
    {
        private Stack<object> _objects = new Stack<object>();

        /// <summary>
        /// 当前正在渲染的对象
        /// </summary>
        public object Target
        {
            get
            {
                return _objects.Count > 0 ? _objects.First() : null;
            }
        }

        public FrameworkTemplate BelongTemplate
        {
            get
            {
                var cell = this.Target as ITemplateCell;
                if (cell == null) return null;
                return cell.BelongTemplate;
            }
        }

        /// <summary>
        /// 当前正在渲染的对象的父对象
        /// </summary>
        public object Parent
        {
            get
            {
                return _objects.Count > 1 ? _objects.ElementAt(1) : null;
            }
        }

        /// <summary>
        /// 追加一个正在渲染的对象
        /// </summary>
        /// <param name="target"></param>
        internal void PushObject(object target)
        {
            _objects.Push(target);
        }

        /// <summary>
        /// 移除当前正在处理的对象
        /// </summary>
        internal void PopObject()
        {
            _objects.Pop();
        }

        private RenderContext()
        {
        }

        private void Clear()
        {
            _objects.Clear();
        }


        #region 基于当前应用程序回话的数据上下文

        /// <summary>
        /// 正处于加载阶段
        /// </summary>
        /// <returns></returns>
        public static bool IsRendering
        {
            get
            {
                return Current != null;
            }
        }

        private const string _sessionKey = "__XamlRenderContext";


        public static RenderContext Current
        {
            get
            {
                var context = AppSession.GetOrAddItem<RenderContext>(_sessionKey,()=> { return null; });
                if (context == null) throw new InvalidOperationException("__XamlRenderContext 为null,无法使用渲染上下文");
                return context;
            }
            internal set
            {
                AppSession.SetItem(_sessionKey, value);
            }
        }

        #endregion

        private static Pool<RenderContext> _pool = new Pool<RenderContext>(() =>
        {
            return new RenderContext();
        }, (ctx, phase) =>
        {
            ctx.Clear();
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 1800 //停留时间60分钟
        });

        public static IPoolItem<RenderContext> Borrow()
        {
            return _pool.Borrow();
        }

    }
}
