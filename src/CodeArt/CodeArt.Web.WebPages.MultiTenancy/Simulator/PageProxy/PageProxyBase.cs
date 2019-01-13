using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Web.Compilation;

using System.Web;
using System.IO;

using CodeArt.Util;
using CodeArt.Runtime;
using CodeArt.Log;

using CodeArt.AOP;
using CodeArt.Concurrent;
using CodeArt.DTO;

namespace CodeArt.Web.WebPages.MultiTenancy
{
    /// <summary>
    /// 对于每个租户会有唯一的页面模拟器，当页面模拟器被创建时，会初始化环境设置，以后所有的请求都是在该环境中执行的
    /// </summary>
    [SafeAccess]
    public abstract class PageProxyBase : MarshalByRefObject, IPageProxy, IDisposable
    {
        public PageProxyBase()
        {
        }

        /// <summary>
        /// 初始化环境设置，以后所有的请求都是在该环境中执行的
        /// </summary>
        public abstract void Initialize(string tenancyId);

        public abstract void Dispose();


        public string Process(string contextCode)
        {
            string code = null;
            try
            {
                using (var temp = TenancyContextPool.Borrow())
                {
                    var context = temp.Item;
                    context.Initialize(contextCode);
                    TenancyContext.Current = context;

                    code = Process(context);

                    TenancyContext.Current = null;
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return code;
        }

        protected abstract string Process(TenancyContext context);

    }
}
