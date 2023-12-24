using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeArt.Web.WebPages.MultiTenancy
{
    public interface ITenancyManager
    {
        ITenancy Get(WebPageContext context);

        /// <summary>
        /// 重启租户
        /// </summary>
        /// <param name="id"></param>
        void Restart(string id);

        /// <summary>
        /// 移除租户
        /// </summary>
        /// <param name="id"></param>
        void Remove(string id);

    }
}
