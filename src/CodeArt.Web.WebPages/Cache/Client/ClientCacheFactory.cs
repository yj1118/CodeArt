using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;


namespace CodeArt.Web.WebPages
{
    public static class ClientCacheFactory
    {
        public static IClientCache Create(WebPageContext context)
        {
            IClientCache clientCache = null;
            var config = WebPagesConfiguration.Global.PageConfig;
            if (config == null || config.ClientCacheFactory == null)
            {
                //没有配置文件设置，那么查看程序集级别的注入
                clientCache = InjectionClientCacheFactory.Instance.Create(context);
            }
            else
            {
                IClientCacheFactory factory = config.ClientCacheFactory.GetInstance<IClientCacheFactory>();
                clientCache = factory.Create(context);
                if (clientCache == null) clientCache = InjectionClientCacheFactory.Instance.Create(context); //配置文件没有设置的就由系统自动设置
            }
            return clientCache;
        }

        public static void Register(string extension, IClientCache cache)
        {
            InjectionClientCacheFactory.Instance.RegisterCache(extension, cache);
        }

    }
}
