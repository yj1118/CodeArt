using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Util;
using CodeArt.Runtime;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 组件内容加载器
    /// </summary>
    internal sealed class PropertiesLoader
    {
        private static PropertiesLoader Instance = new PropertiesLoader();

        public static void Load(object obj, HtmlNode objNode)
        {
            Instance.LoadProperties(obj, objNode);
        }


        private void LoadProperties(object obj, HtmlNode objNode)
        {
            LoadSimple(obj, objNode);
            LoadElement(obj, objNode);
        }

        #region 加载简单属性

        /// <summary>
        /// 加载简单属性
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="objNode"></param>
        /// <param name="context"></param>
        private void LoadSimple(object obj, HtmlNode objNode)
        {
            var attributes = objNode.Attributes;
            if (attributes.Count == 0) return;

            var ctx = LoadContext.Current;

            var objType = obj.GetType();

            //我们优先处理xamlStyle，然后在处理其他的加载操作
            foreach (var attr in attributes)
            {
                if (string.Equals(attr.Name, FrameworkElement.XamlStyleProperty.Name, StringComparison.OrdinalIgnoreCase))
                {
                    LoadSimple(ctx, objType, obj, attr);
                }
            }

            //这里就不再重复处理xamlStyle
            foreach (var attr in attributes)
            {
                if (!string.Equals(attr.Name, FrameworkElement.XamlStyleProperty.Name, StringComparison.OrdinalIgnoreCase))
                {
                    LoadSimple(ctx, objType, obj, attr);
                }
            }
        }

        /// <summary>
        /// 这是一个重构方法，内部使用
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="objType"></param>
        /// <param name="obj"></param>
        /// <param name="attr"></param>
        private void LoadSimple(LoadContext ctx, Type objType, object obj, HtmlAttribute attr)
        {
            if (IsSimpleProperty(attr))
            {
                var propertyName = attr.OriginalName;

                ctx.PushProperty(propertyName);
                LoadSingleSimple(objType, obj, propertyName, attr.Value);
                ctx.PopProperty();
            }
            else if (IsInnerProperty(attr))
            {
                var innerName = attr.OriginalName;
                ctx.PushProperty(innerName);
                LoadInnerProperty(ctx, obj, innerName, attr.Value);
                ctx.PopProperty();
            }
        }

        private bool IsInnerProperty(HtmlAttribute attr)
        {
            var name = attr.OriginalName;
            return name.IndexOf(".") != -1;
        }

        private void LoadInnerProperty(LoadContext ctx, object obj, string innerName, string attrValue)
        {
            var current = obj;
            var propertyNames = innerName.Split('.');
            var lastIndex = propertyNames.Length - 1;
            for (var i = 0; i < propertyNames.Length; i++)
            {
                var propertyName = propertyNames[i];
                if (i == lastIndex)
                {
                    ctx.PushObject(current);
                    ctx.PushProperty(propertyName);
                    LoadSingleSimple(current.GetType(), current, propertyName, attrValue);
                    ctx.PushProperty(propertyName);
                    ctx.PopObject();
                }
                else
                {
                    var target = RuntimeUtil.GetPropertyValue(current, propertyName);
                    if (target == null)
                    {
                        throw new XamlException("类型" + current.GetType().FullName + "的属性" + propertyName + "值为null,无法赋值");
                    }
                    current = target;
                }
            }
        }


        private bool IsSimpleProperty(HtmlAttribute attr)
        {
            var name = attr.OriginalName;
            return name.IndexOf(".") == -1;
        }

        /// <summary>
        /// 加载单个简单属性
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="attr"></param>
        private static void LoadSingleSimple(Type objType, object obj, string propertyName, string attrValue)
        {
            if (propertyName.Equals("xmlns", StringComparison.OrdinalIgnoreCase)
                || propertyName.StartsWith("xmlns:", StringComparison.OrdinalIgnoreCase)
                || propertyName.Equals("x:key", StringComparison.OrdinalIgnoreCase))
                return;
            if (propertyName.Equals("x:Name", StringComparison.OrdinalIgnoreCase))
                propertyName = "name";

            var propertyInfo = objType.ResolveProperty(propertyName);
            if (propertyInfo == null)
            {
                var element = obj as IHtmlElementCore;
                if (element == null)
                    throw new XamlException("类型" + objType.FullName + "没有实现" + typeof(IHtmlElementCore).FullName + "，不能使用自定义属性");

                var e = CreateMarkupExtension(attrValue);
                if (e != null)
                {
                    var value = e.ProvideValue(obj, propertyName);
                    element.Attributes.SetValue(obj, propertyName, value);
                }
                else
                    element.Attributes.SetValue(obj, propertyName, attrValue);
            }
            else
            {
                var d = obj as DependencyObject;
                var e = CreateMarkupExtension(attrValue);
                if (e != null)
                {
                    if (d == null) throw new XamlException("类型" + objType.FullName + "不是依赖对象，不能应用标记扩展");
                    //标记扩展
                    var dp = DependencyProperty.GetProperty(objType, propertyInfo.Name);
                    if (dp == null) throw new XamlException("类型" + objType.FullName + "的属性" + propertyName + "不是依赖属性，不能应用标记扩展");
                    var value = e.ProvideValue(obj, dp);
                    d.SetValue(dp, value);
                }
                else
                {
                    //根据字符串的值，转换成具体类型赋值
                    var value = GetValue(attrValue, propertyInfo);
                    propertyInfo.SetValue(obj, value);
                }
            }
        }


        private static TypeConverter GetConverter(PropertyInfo propertyInfo)
        {
            //先查找属性是否定义了类型转换器，如果没有定义，那么从属性所属的类型上查找是否定义了类型转换器
            var attr = propertyInfo.GetCustomAttribute<TypeConverterAttribute>(true) ?? propertyInfo.PropertyType.GetCustomAttribute<TypeConverterAttribute>(true);
            if (attr == null) return PrimitiveTypeConverter.Instance; //如果没有定义，那么使用默认的转换器
            return SafeAccessAttribute.CreateInstance<TypeConverter>(attr.ConverterType);
        }

        /// <summary>
        /// 将文本值value转换为propertyInfo的实际类型的值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static object GetValue(string value, PropertyInfo propertyInfo)
        {
            var converter = GetConverter(propertyInfo);
            var targetType = propertyInfo.PropertyType;
            return converter.ConvertTo(value, targetType);
        }

        #endregion

        #region 加载属性元素

        /// <summary>
        /// 加载属性元素
        /// </summary>
        private void LoadElement(object obj, HtmlNode objNode)
        {
            var childNodes = GetPropertyElementNodes(objNode);
            if (childNodes.Count == 0) return;

            var objType = obj.GetType();

            foreach (var node in childNodes)
            {
                var pos = node.OriginalName.IndexOf(".");
                var objName = node.OriginalName.Substring(0, pos);
                var propertyName = node.OriginalName.Substring(pos + 1);
                if (!objName.Equals(objNode.OriginalName)) throw new XamlException("元素属性定义错误" + node.OriginalName);

                var propertyInfo = objType.ResolveProperty(propertyName);
                if (propertyInfo == null) throw new WebException("在类型" + objType.FullName + "中没有找到属性" + propertyName);

                var propertyNodes = ContentLoader.GetPropertyNodes(propertyInfo, node);
                ContentLoader.SetPropertyValue(obj, propertyInfo, propertyNodes);
            }
        }

        /// <summary>
        /// 获得属性元素节点
        /// </summary>
        /// <returns></returns>
        private static IList<HtmlNode> GetPropertyElementNodes(HtmlNode objNode)
        {
            var childNodes = objNode.TrimChildNodes(true);
            List<HtmlNode> nodes = new List<HtmlNode>();
            foreach (var child in childNodes)
            {
                if (IsPropertyElement(child))
                    nodes.Add(child);
            }
            return nodes;
        }


        /// <summary>
        /// 节点是否为属性元素
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        internal static bool IsPropertyElement(HtmlNode node)
        {
            return node.NodeType == HtmlNodeType.Element && node.OriginalName.IndexOf(".") > -1;
        }

        #endregion


        #region 解析标记扩展

        private static MarkupExtension CreateMarkupExtension(string attrValue)
        {
            return MarkupExtensionFactory.Create(attrValue);
        }

        #endregion


        /// <summary>
        /// 该方法可以识别attrValue的类型（动态标记或实际值），并将其值设置到对象d的dp属性上
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        /// <param name="propertyInfo">属性信息,由于有可能需要外界指定属性信息，所以我们公开这个设置，例如：XamlStyle->Setter对象，这个对象的Value的属性值由TargetType决定，而不是Setter对象的Value类型决定</param>
        /// <param name="attrValue"></param>
        internal static void SetValue(DependencyObject d, DependencyProperty dp, PropertyInfo propertyInfo, string attrValue)
        {
            var e = PropertiesLoader.CreateMarkupExtension(attrValue);
            object value = null;
            if (e != null)
            {
                value = e.ProvideValue(d, dp);
            }
            else
            {
                value = GetValue(attrValue, propertyInfo);
            }
            d.SetValue(dp, value);
        }

        /// <summary>
        /// 设置属性的值
        /// </summary>
        /// <param name="d"></param>
        /// <param name="dp"></param>
        /// <param name="attrValue"></param>
        internal static void SetValue(DependencyObject d, DependencyProperty dp, string attrValue)
        {
            var objType = d.GetType();
            var propertyInfo = objType.ResolveProperty(dp.Name);
            SetValue(d, dp, propertyInfo, attrValue);
        }



    }
}
