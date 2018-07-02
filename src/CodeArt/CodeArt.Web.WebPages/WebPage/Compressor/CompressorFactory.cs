using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;


namespace CodeArt.Web.WebPages
{
    public static class CompressorFactory
    {
        public static ICompressor Create(WebPageContext context)
        {
            ICompressor cpor = null;
            var config = WebPagesConfiguration.Global.PageConfig;
            if (config == null || config.CompressorFactory == null)
            {
                //没有配置文件设置，那么查看程序集级别的注入
                cpor = InjectionCompressorFactory.Instance.Create(context);
            }
            else
            {
                var factory = config.CompressorFactory.GetInstance<ICompressorFactory>();
                cpor = factory.Create(context);
                if (cpor == null) cpor = InjectionCompressorFactory.Instance.Create(context); //配置文件没有设置的就由系统自动设置
            }
            return cpor;
        }

        public static void Register(string extension, ICompressor compressor)
        {
            InjectionCompressorFactory.Instance.Register(extension, compressor);
        }

    }
}
