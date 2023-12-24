using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web;

using CodeArt;
using CodeArt.IO;

namespace CodeArt.Web.WebPages.MultiTenancy
{
    /// <summary>
    /// 基于应用程序域的租户
    /// </summary>
    public abstract class AppDomainTenancy : TenancyBase
    {
        public AppDomainTenancy(string id)
            : base(id)
        {

        }

        public override IPageProxy GetPage()
        {
            return _proxy;
        }


        private AppDomain _appDomain;
        private IPageProxy _proxy;
        private object syncObject = new object();

        public override void Install()
        {
            lock(syncObject)
            {
                if (_appDomain != null) return;
                try
                {
                    var domainName = string.Format("tenancy #{0}", this.Id);

                    _appDomain = CreateAppDomain(domainName);

                    var proxyType = GetPageProxyType();
                    _proxy = (IPageProxy)_appDomain.CreateInstanceAndUnwrap(Assembly.GetAssembly(proxyType).FullName, proxyType.FullName);
                    _proxy.Initialize(this.Id);
                }
                catch (Exception ex)
                {
                    if (_appDomain != null)
                        AppDomain.Unload(_appDomain);

                    throw ex;
                }
            }
        }

        protected abstract AppDomain CreateAppDomain(string domainName);


        /// <summary>
        /// 获得页面代理的类型，该类型会运行在一个独立的appDomain中
        /// </summary>
        /// <returns></returns>
        protected abstract Type GetPageProxyType();


        public override void Uninstall()
        {
            lock (syncObject)
            {
                if (_proxy != null)
                    _proxy.Dispose();

                if (_appDomain != null)
                    AppDomain.Unload(_appDomain);
            }
        }

        public override void Restart()
        {
            Uninstall();
            Install();
        }

    }
}
