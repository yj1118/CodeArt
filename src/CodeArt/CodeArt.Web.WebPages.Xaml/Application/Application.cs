using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Web.WebPages.Xaml.Markup;

namespace CodeArt.Web.WebPages.Xaml
{
    public class Application : IHtmlElementCore
    {
        public HybridDictionary Resources
        {
            get;
            set;
        }

        public object FindResource(object resourceKey)
        {
            var result = this.Resources[resourceKey];
            if (result != null)
            {
                var d = result as DependencyObject;
                if (d != null)
                {
                    d.LoadPinned(); //在使用资源之前，需要加载资源的固化值
                }
                return result;
            }
            throw new XamlException("没有找到资源" + resourceKey + "的定义");
        }

        public CustomAttributeCollection Attributes
        {
            get;
            private set;
        }

        #region html附件

        public ExternalLink FindLink(string sourceKey)
        {
            var result = this.Links[sourceKey] as ExternalLink;
            return result;
        }

        public HybridDictionary Links
        {
            get;
            set;
        }

        /// <summary>
        /// 资产版本号
        /// </summary>
        public string AssetVersion
        {
            get;
            set;
        }

        #endregion

        public Application()
        {
            this.Attributes = new CustomAttributeCollection();
            this.Resources = new HybridDictionary();
            this.Links = new HybridDictionary();
            this.AssetVersion = "1";
        }


        private static object _syncObject = new object();

        private static Application _current = null;

        public static Application Current
        {
            get
            {
                if(_current == null)
                {
                    lock(_syncObject)
                    {
                        if (_current == null)
                        {
                            const string appPath = "/app.htm";
                            _current = XPCFactory.Create(appPath) as Application ?? new Application();
                            ImportExternalLinks();
                        }
                    }
                }
                return _current;
            }
        }


        #region 注入外部链接

        private static Dictionary<string, ExternalLink> _importExternalLinks = new Dictionary<string, ExternalLink>();

        public static void ImportExternalLink(string key, string src)
        {
            if (!_importExternalLinks.ContainsKey(key))
                _importExternalLinks.Add(key, new ExternalLink() { Src = src });
        }

        private static void ImportExternalLinks()
        {
            var links = _current.Links;

            foreach (var p in _importExternalLinks)
            {
                if (!links.Contains(p.Key))
                {
                    links.Add(p.Key, p.Value);
                }
            }
        }

        #endregion
    }
}
