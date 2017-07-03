using System;
using System.Collections.Generic;
using System.Threading;
using System.Web;

using CodeArt.AppSetting;
using CodeArt.Concurrent;

namespace CodeArt.Web
{
    /// <summary>
    /// 当是web环境时，使用WebAppSession，否则使用ThreadSession
    /// </summary>
    [AppSessionAccess]
    public class CombineAppSession : IAppSession
    {
        public bool Initialized
        {
            get
            {
                return  HttpContext.Current == null ? ThreadSession.Instance.Initialized
                                                    : WebAppSession.Instance.Initialized;
            }
        }

        public void Initialize()
        {
            if (HttpContext.Current == null) ThreadSession.Instance.Initialize();
            else WebAppSession.Instance.Initialize();
        }

        public void Dispose()
        {
            if (HttpContext.Current == null) ThreadSession.Instance.Dispose();
            else WebAppSession.Instance.Dispose();
        }


        public object GetItem(string name)
        {
            if (HttpContext.Current == null) return ThreadSession.Instance.GetItem(name);
            return WebAppSession.Instance.GetItem(name);
        }

        public bool ContainsItem(string name)
        {
            if (HttpContext.Current == null) return ThreadSession.Instance.ContainsItem(name);
            return WebAppSession.Instance.ContainsItem(name);
        }

        public void SetItem(string name, object value)
        {
            if (HttpContext.Current == null) ThreadSession.Instance.SetItem(name, value);
            else WebAppSession.Instance.SetItem(name, value);
        }

        public CombineAppSession() { }

        internal static readonly CombineAppSession Instance = new CombineAppSession();



    }
}
