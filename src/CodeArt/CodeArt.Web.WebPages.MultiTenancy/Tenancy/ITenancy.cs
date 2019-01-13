using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Web;

namespace CodeArt.Web.WebPages.MultiTenancy
{
    public interface ITenancy
    {
        /// <summary>
        /// 租户的唯一标示
        /// </summary>
        string Id { get; }

        IPageProxy GetPage();


        /// <summary>
        /// 可以安装租户
        /// </summary>
        void Install();

        /// <summary>
        /// 可以卸载租户
        /// </summary>
        void Uninstall();

        /// <summary>
        /// 可以重启
        /// </summary>
        void Restart();

    }
}
