using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CodeArt.Runtime;

using HtmlAgilityPack;
using CodeArt.HtmlWrapper;
using CodeArt.Util;

namespace CodeArt.Web.WebPages.Xaml.Markup
{
    /// <summary>
    /// 组件内容加载器
    /// </summary>
    internal sealed class ContentLoader
    {
        private static ContentLoader Instance = new ContentLoader();

        public static void Load(object obj, HtmlNode objNode)
        {
            Instance.LoadContent(obj, objNode);
        }

        /// <summary>
        /// 获取对象内容属性的值
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object GetPropertyValue(object obj)
        {
            var propertyInfo = Instance.GetContentProperty(obj);
            if (propertyInfo == null) return null;
            return propertyInfo.GetValue(obj);
        }

        private ContentLoader() { }

        private void LoadContent(object obj, HtmlNode objNode)
        {
            var objType = obj.GetType();
            bool ignoreBlank = IgnoreBlankAttribute.GetAttribute(objType).Ignore;

            var childNodes = GetContentNodes(objNode, ignoreBlank);
            if (childNodes.Count == 0) return;

            if (TrySetList(obj, childNodes, obj)) return;
            if (TrySetDictionary(obj, childNodes, obj)) return;


            //设置内容属性
            var propertyInfo = GetContentProperty(obj);
            if (propertyInfo == null) return;

            bool propertyIgnorBlank = IgnoreBlankAttribute.GetAttribute(propertyInfo).Ignore;
            if (ignoreBlank != propertyIgnorBlank) childNodes = GetContentNodes(objNode, propertyIgnorBlank);

            SetPropertyValue(obj, propertyInfo, childNodes);
        }

        /// <summary>
        /// 设置属性的值
        /// 如果属性的类型是IList或IDictionary,那么将子节点映射的对象作为集合成员加入
        /// 否则，将子节点映射的对象设置为属性值
        /// 该算法在加载元素属性和内容属性时会用到，两者的加载算法一致
        /// </summary>
        internal static void SetPropertyValue(object owner, PropertyInfo propertyInfo, IList<HtmlNode> propertyNodes)
        {
            ArgumentAssert.IsNotNull(propertyInfo, "propertyInfo");

            var ctx = LoadContext.Current;
            ctx.PushProperty(propertyInfo.Name);

            var propertyValue = propertyInfo.GetValue(owner);
            if ((propertyInfo.PropertyType.IsImplementOrEquals(typeof(IList))
                || propertyInfo.PropertyType.IsImplementOrEquals(typeof(IDictionary)))
                && propertyValue == null) throw new WebException("集合类型的属性" + owner.GetType().Name + "." + propertyInfo.Name + "的默认值不能为null");

            if (!TrySetList(propertyValue, propertyNodes, owner))
            {
                if (!TrySetDictionary(propertyValue, propertyNodes, owner))
                {
                    //最后作为对象设置
                    if (propertyNodes.Count != 1)
                        throw new WebException("属性" + owner.GetType().Name + "." + propertyInfo.Name + "不是集合，xaml的根元素不能超过1");
                    propertyValue = ComponentLoader.CreateComponent(propertyNodes[0], owner);
                    propertyInfo.SetValue(owner, propertyValue);

                    //var e = propertyValue as UIElement;
                    //if (e != null) e.SetValue(UIElement.ParentProperty, owner);
                }
            }

            ctx.PopProperty();
        }

        internal static IList<HtmlNode> GetPropertyNodes(PropertyInfo propertyInfo, HtmlNode ownerNode)
        {
            bool propertyIgnorBlank = IgnoreBlankAttribute.GetAttribute(propertyInfo).Ignore;
            return GetContentNodes(ownerNode, propertyIgnorBlank);
        }

        /// <summary>
        /// 获得内容节点
        /// </summary>
        /// <returns></returns>
        private static IList<HtmlNode> GetContentNodes(HtmlNode objNode, bool ignoreBlank)
        {
            var childNodes = objNode.TrimChildNodes(ignoreBlank);
            List<HtmlNode> nodes = new List<HtmlNode>();
            foreach (var child in childNodes)
            {
                if (PropertiesLoader.IsPropertyElement(child)) continue;
                nodes.Add(child);
            }
            return nodes;
        }


        private static bool TrySetList(object obj, IList<HtmlNode> childNodes, object parent)
        {
            var list = obj as IList;
            if (list != null)
            {
                foreach (var childNode in childNodes)
                {
                    var child = ComponentLoader.CreateComponent(childNode, parent);
                    list.Add(child);
                }
                return true;
            }
            return false;
        }

        private static bool TrySetDictionary(object obj, IList<HtmlNode> childNodes, object parent)
        {
            var dictionary = obj as IDictionary;
            if (dictionary != null)
            {
                foreach (var childNode in childNodes)
                {
                    var key = childNode.GetAttributeValue("x:key", string.Empty);
                    if (string.IsNullOrEmpty(key)) throw new WebException("没有为节点指定x:key属性" + childNode.OuterHtml);
                    var child = ComponentLoader.CreateComponent(childNode, parent);
                    dictionary.Add(key, child);
                }
                return true;
            }
            return false;
        }


        /// <summary>
        /// 获取对象的内容属性对应的属性信息
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private PropertyInfo GetContentProperty(object obj)
        {
            var objType = obj.GetType();
            var contentProperty = ContentPropertyAttribute.GetAttribute(objType);
            //if (contentProperty == null) throw new WebException("类型" + objType.FullName + "没有指定" + typeof(ContentPropertyAttribute).FullName + "特性");
            if (contentProperty == null) return null;
            //var dependencyObject = obj as DependencyObject;
            //if (dependencyObject == null) throw new WebException("类型" + objType.FullName + "没有继承自" + typeof(DependencyObject).FullName + "无法使用" + typeof(ContentPropertyAttribute).FullName + "特性");
            var propertyInfo = objType.ResolveProperty(contentProperty.Name);
            if (propertyInfo == null) throw new WebException("类型" + objType.FullName + "中没有找到属性" + contentProperty.Name + "的定义");
            return propertyInfo;
        }

    }
}
