using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.AppSetting;

namespace CodeArt.Web.WebPages.Xaml
{

    //public sealed class RenderContext
    //{
    //    private RenderContext() { }

    //    public void Clear()
    //    {
    //        _objects.Clear();
    //    }

    //    private Stack<RenderObject> _objects = new Stack<RenderObject>();

    //    /// <summary>
    //    /// 当前正在渲染的对象
    //    /// </summary>
    //    public object Target
    //    {
    //        get
    //        {
    //            return _objects.Count > 0 ? _objects.First().Target : null;
    //        }
    //    }


    //    /// <summary>
    //    /// 追加一个正在渲染的对象
    //    /// </summary>
    //    /// <param name="target"></param>
    //    public void Push(object target)
    //    {
    //        _objects.Push(new RenderObject(target));
    //    }

    //    /// <summary>
    //    /// 移除当前正在渲染的对象
    //    /// </summary>
    //    public void Pop()
    //    {
    //        _objects.Pop();
    //    }


    //    #region 基于当前应用程序回话的数据上下文


    //    private const string _sessionKey = "__XamlRenderContext.Current";


    //    public static RenderContext Current
    //    {
    //        get
    //        {
    //            var renderContext = WebAppSession.GetOrAddItem<RenderContext>(
    //                _sessionKey,
    //                () =>
    //                {
    //                    return new RenderContext();
    //                });
    //            if (renderContext == null) throw new InvalidOperationException("__XamlRenderContext.Current为null,无法使用渲染上下文");
    //            return renderContext;
    //        }
    //    }

    //    #endregion

    //}
}
