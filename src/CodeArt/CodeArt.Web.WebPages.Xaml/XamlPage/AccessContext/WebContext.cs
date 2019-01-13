using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;
using CodeArt.Web.WebPages;

namespace CodeArt.Web.WebPages.Xaml
{
    /// <summary>
    /// 基于web的访问时环境
    /// </summary>
    [SafeAccess]
    public class WebContext : IAccessContext
    {
        private WebContext() { }


        public static readonly WebContext Instance = new WebContext();

        public bool IsGET => WebPageContext.Current.IsGET;

        public string VirtualPath => WebPageContext.Current.VirtualPath;

        public bool IsMobileDevice => WebPageContext.Current.IsMobileDevice;

        public T GetQueryValue<T>(string name, T defaultValue)
        {
            return WebPageContext.Current.GetQueryValue<T>(name, defaultValue);
        }
    }
}
