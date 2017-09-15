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
            var entries = _pool.Borrow();
            entries.Initializ();
            HttpContext.Current.Items[ContentEntries.Name] = entries;
        }

        public void Dispose()
        {
            var item = GetEntries();
            _pool.Return(item);
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


        private static PoolWrapper<ContentEntries> _pool = new PoolWrapper<ContentEntries>(() =>
        {
            return new ContentEntries();
        }, (obj, phase) =>
        {
            if (phase == PoolItemPhase.Returning)
            {
                obj.Clear();
            }
            return true;
        }, new PoolConfig()
        {
            MaxRemainTime = 300 //闲置时间300秒
        });
    }
}
