using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using CodeArt.IO;
using CodeArt.Util;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Web.WebPages.Xaml.Markup;
using CodeArt.Web.WebPages.Xaml.Controls;
using CodeArt.Concurrent;
using System.Text;

namespace CodeArt.Web.WebPages.Xaml
{
    public static class XamlUtil
    {
        private const string _xamlDeclare = "<!DOCTYPE xaml>";

        internal static bool IsDeclareXaml(string xaml)
        {
            return xaml.StartsWith(_xamlDeclare, StringComparison.OrdinalIgnoreCase);
        }

        internal static HtmlNode GetNode(string xaml)
        {
            if (IsDeclareXaml(xaml)) xaml = xaml.Substring(_xamlDeclare.Length);

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(xaml);
            var nodes = doc.DocumentNode.TrimChildNodes(true);
            if (nodes.Count == 1) return nodes[0];
            throw new WebException("xaml格式不正确，根节点数不为1");
        }

        /// <summary>
        /// 节点是否为元素属性
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static bool IsElementProperty(this HtmlNode node)
        {
            if (node == null || node.ParentNode == null) return false;
            var name = node.OriginalName;
            var dotPos = name.IndexOf(".");
            if (dotPos < 0) return false;
            //进一步判断
            var parentName = name.Substring(0, dotPos);
            return parentName.Equals(node.ParentNode.OriginalName, StringComparison.Ordinal) && name.Substring(dotPos + 1).IndexOf(".") > 0;
        }

        /// <summary>
        /// 是否为模板类型组件
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static bool IsTemplateComponent(this HtmlNode node)
        {
            var componentType = node.MapComponentType();
            if (componentType == null) return false;
            return componentType.IsSubclassOf(typeof(FrameworkTemplate));
        }

        /// <summary>
        /// 节点映射的组件类型
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static Type MapComponentType(this HtmlNode node)
        {
            return ComponentTypeLocator.Locate(node) ?? HtmlElement.Type;
        }

        /// <summary>
        /// 获取对象类型（优化性能）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static Type GetObjectType(object obj)
        {
            var d = obj as DependencyObject;
            if (d != null) return d.ObjectType;
            return obj.GetType();
        }

        /// <summary>
        /// 获得元素的定义名称
        /// 1.没有实际的类型定义，定义名称无效(该条规则已取消)
        /// 2.元素属性定义名称无效
        /// 3.元素是模板元素的子节点，那么定义名称无效
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static string GetDeclareName(this HtmlNode node)
        {
            //if (node.MapComponentType() == null) return null; //没有实际的类型定义，定义名称无效
            var name = node.GetAttributeValue("name", null);
            var xName = node.GetAttributeValue("x:name", null);
            if (name != null && xName != null) throw new WebException("不能同时定义x:Name和Name属性");
            var declareName = name ?? xName;
            if (string.IsNullOrEmpty(declareName)) return null;
            if (node.IsElementProperty()) return null;  //是元素属性，定义名称无效

            var parent = node.ParentNode;
            while (parent != null && parent.NodeType != HtmlNodeType.Document)
            {
                if (parent.IsTemplateComponent()) return null; //元素是模板元素的子节点，那么定义名称无效
                parent = parent.ParentNode;
            }
            return declareName;
        }

        /// <summary>
        /// 回收站点内存
        /// </summary>
        internal static void RecoverMemory()
        {
            string fileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.config");
            var text = File.ReadAllText(fileName);
            File.WriteAllText(fileName, text);
        }

        public readonly static Pool<PageBrush> BrushPool = new Pool<PageBrush>(() =>
        {
            return new PageBrush();
        }, (brush, phase) =>
        {
            brush.Clear();
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 1800 //停留时间60分钟
        });

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xaml"></param>
        /// <param name="onlyMain">是否仅输出主体代码，如果该选项为true，那么当代码段没有body节点时，是不会输出script,style的代码的</param>
        /// <returns></returns>
        public static string GetCode(string xaml, bool onlyMain)
        {
            var obj = XamlReader.ReadComponent(xaml) as UIElement;
            return GetCode(obj, onlyMain);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="element"></param>
        /// <param name="onlyMain">是否仅输出主体代码，如果该选项为true，那么当代码段没有body节点时，是不会输出script,style的代码的</param>
        /// <returns></returns>
        public static string GetCode(UIElement element, bool onlyMain)
        {
            if(RenderContext.IsRendering)
            {
                string code = string.Empty;
                using (var temp = XamlUtil.BrushPool.Borrow())
                {
                    var brush = temp.Item;
                    element.Render(brush);
                    code = brush.GetCode(onlyMain);
                }
                return code;
            }
            else
            {
                string code = string.Empty;
                UsingRender((brush) =>
                {
                    element.OnLoad();
                    element.Render(brush);
                    code = brush.GetCode(onlyMain);
                });
                return code;
            }
        }

        /// <summary>
        /// 使用渲染环境,在渲染环境下，数据的操作都是本地的
        /// </summary>
        /// <param name="action"></param>
        public static void UsingRender(Action<PageBrush> action)
        {
            using (var temp = XamlUtil.BrushPool.Borrow())
            {
                var brush = temp.Item;
                using (var ctx = RenderContext.Borrow())
                {
                    RenderContext.Current = ctx.Item;
                    RenderContext.Current.RootBrush = brush;
                    try
                    {
                        if (action != null) action(brush);
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                    finally
                    {
                        RenderContext.Current = null;
                    }
                }
            }
        }


        public static void OutputAttributes(CustomAttributeCollection attributes, StringBuilder code)
        {
            foreach (var item in attributes)
            {
                var attr = item as CustomAttribute;
                OutputAttribute(attr, code);
            }
        }

        public static void OutputAttribute(CustomAttribute attr, StringBuilder code)
        {
            switch(attr.Name)
            {
                case "view": code.AppendFormat(" data-view=\"{0}\"", ProxyCodeExtend.TidyView(attr.GetPrintValue())); break;
                case "form": code.AppendFormat(" data-form=\"{0}\"", attr.GetPrintValue()); break;
                default:
                    code.AppendFormat(" {0}=\"{1}\"", attr.Name, attr.GetPrintValue());
                    break;
            }
        }

        public static void OutputAttributes(CustomAttributeCollection attributes, PageBrush brush)
        {
            foreach (var item in attributes)
            {
                var attr = item as CustomAttribute;
                OutputAttribute(attr, brush);
            }
        }

        public static void OutputAttribute(CustomAttribute attr, PageBrush brush)
        {
            switch (attr.Name)
            {
                case "view": brush.DrawFormat(" data-view=\"{0}\"", ProxyCodeExtend.TidyView(attr.GetPrintValue())); break;
                case "form": brush.DrawFormat(" data-form=\"{0}\"", attr.GetPrintValue()); break;
                default:
                    brush.DrawFormat(" {0}=\"{1}\"", attr.Name, attr.GetPrintValue());
                    break;
            }
        }

    }

}
