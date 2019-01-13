using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

using CodeArt.AppSetting;
using CodeArt.Concurrent;

namespace CodeArt.Web
{
    /// <summary>
    /// 基于Web技术的应用程序会话状态
    /// </summary>
    [AppSessionAccess]
    internal class WebAppSession : IAppSession
    {
        public bool Initialized
        {
            get
            {
                return HttpContext.Current.Items.Contains(ContentEntries.Name);
            }
        }

        public void Initialize()
        {
            var entries = ContentEntries.Pool.Borrow();
            entries.Initializ();
            HttpContext.Current.Items[ContentEntries.Name] = entries;
        }

        public void Dispose()
        {
            var item = GetEntries();
            ContentEntries.Pool.Return(item);
        }


        private ContentEntries GetEntries()
        {
            return (ContentEntries)HttpContext.Current.Items[ContentEntries.Name];
        }

        public object GetItem(string name)
        {
            return GetEntries().Get(name);
        }

        public bool ContainsItem(string name)
        {
            return GetEntries().Contains(name);
        }

        public void SetItem(string name, object value)
        {
            GetEntries().Set(name, value);
        }

        public WebAppSession() { }

        public static readonly WebAppSession Instance = new WebAppSession();

    }
}
