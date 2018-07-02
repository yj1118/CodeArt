using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

using HtmlAgilityPack;
using CodeArt.AppSetting;

namespace CodeArt.Web.WebPages.Xaml
{
    public sealed class XmlnsDictionary
    {
        private Dictionary<string, string> _data = new Dictionary<string, string>();

        private XmlnsDictionary() { }

        private void Add(string name,string value)
        {
            _data.Add(name, value);
        }

        public bool Contains(string name)
        {
            return _data.ContainsKey(name);
        }

        public void Each(Action<KeyValuePair<string, string>> action)
        {
            foreach (var p in _data)
            {
                action(p);
            }
        }

        /// <summary>
        /// 获取对象的xaml命名空间
        /// </summary>
        /// <param name="componentName"></param>
        /// <returns></returns>
        public string GetXamlNamespace(string componentName)
        {
            var prefixName = GetPrefixName(componentName);
            return GetXamlNamespaceByPrefixName(prefixName);
        }

        public string GetXamlNamespaceByPrefixName(string prefixName)
        {
            var key = string.IsNullOrEmpty(prefixName) ? "xmlns" : string.Format("xmlns:{0}", prefixName);
            string value = null;
            if (!_data.TryGetValue(key, out value)) return null;
            return value;
        }

        private static string GetPrefixName(string elementName)
        {
            if (string.IsNullOrEmpty(elementName)) return string.Empty;
            var p = elementName.IndexOf(":");
            if (p == -1) return string.Empty;
            return elementName.Substring(0, p);
        }

        public bool IsEmpty()
        {
            return _data.Count == 0;
        }

        //public static readonly XmlnsDictionary Empty = new XmlnsDictionary();

        /// <summary>
        /// 新建一个命名空间信息，并追加到栈顶
        /// </summary>
        internal static void New()
        {
            Contexts.Push(new XmlnsDictionary());
        }

        internal static void Pop()
        {
            Contexts.Pop();
        }


        internal static XmlnsDictionary Current
        {
            get
            {
                var ctxs = Contexts;
                if (ctxs.Count == 0) New();
                return ctxs.First();
            }
        }


        private const string _sessionKey = "__XmlnsDictionary.Contexts";

        private static Stack<XmlnsDictionary> Contexts
        {
            get
            {
                var contexts = AppSession.GetOrAddItem<Stack<XmlnsDictionary>>(
                    _sessionKey,
                    () =>
                    {
                        return new Stack<XmlnsDictionary>();
                    });
                if (contexts == null) throw new InvalidOperationException("__XmlnsDictionary.Contexts 为null,无法使用XmlnsDictionary上下文");
                return contexts;
            }
        }


        /// <summary>
        /// 从节点中收集命名空间信息
        /// </summary>
        /// <param name="node"></param>
        internal static XmlnsDictionary Collect(HtmlNode node)
        {
            XmlnsDictionary xmlns = Current;
            if (node == null) return xmlns;
            var attributes = node.Attributes;
            if (attributes.Count > 0)
            {
                foreach (var attr in attributes)
                {
                    if (attr.Name != null && (attr.Name.Equals("xmlns", StringComparison.OrdinalIgnoreCase) || attr.Name.StartsWith("xmlns:")))
                    {
                        if (xmlns.Contains(attr.Name)) continue;
                        xmlns.Add(attr.Name, attr.Value);
                    }
                }
            }
            return xmlns;
        }

    }

}
