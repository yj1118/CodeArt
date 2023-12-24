using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CodeArt.Concurrent;

namespace CodeArt.Web.WebPages.MultiTenancy
{
    public static class TenancyManager
    {
        /// <summary>
        /// 这里要获得对应的租户
        /// </summary>
        /// <param name="virtualPath"></param>
        /// <returns></returns>
        internal static ITenancy GetTenancy(WebPageContext context)
        {
            if (_manager == null) throw new InvalidOperationException("没有定义ITenancyFactory的实现");
            return _manager.Get(context);
        }


        /// <summary>
        /// 租户工厂，请保证<paramref name="factory"/>是单例的
        /// </summary>
        /// <param name="factory"></param>
        public static void RegisterRouter(ITenancyManager manager)
        {
            SafeAccessAttribute.CheckUp(manager.GetType());
            _manager = manager;
        }

        private static ITenancyManager _manager = null;


    }
}
