using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Web;

namespace CodeArt.Web.WebPages.MultiTenancy
{
    /// <summary>
    /// 基于应用程序域的租户
    /// </summary>
    public abstract class TenancyBase : ITenancy
    {
        public string Id
        {
            get;
            private set;
        }

        public TenancyBase(string id)
        {
            this.Id = id;
        }

        public abstract IPageProxy GetPage();


        public abstract void Install();

        public abstract void Uninstall();

        public abstract void Restart();

    }
}
