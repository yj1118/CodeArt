using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using CodeArt.Web;


namespace CodeArt.Web.WebPages
{
    /// <summary>
    /// 注入式的压缩工厂
    /// </summary>
    internal class InjectionCompressorFactory : ICompressorFactory
    {
        private InjectionCompressorFactory() { }

        public static InjectionCompressorFactory Instance = new InjectionCompressorFactory();

        private static Dictionary<string, ICompressor> _compressors = new Dictionary<string, ICompressor>(5);

        /// <summary>
        /// 注册压缩处理器，请保证compressor是单例的
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="compressor"></param>
        public void Register(string extension, ICompressor compressor)
        {
            if (!_compressors.ContainsKey(extension))
            {
                lock (_compressors)
                {
                    if (!_compressors.ContainsKey(extension))
                    {
                        if (extension.StartsWith(".")) extension = extension.Substring(1);
                        _compressors.Add(extension, compressor);
                        _compressors.Add(string.Format(".{0}", extension), compressor);
                    }
                }
            }
        }

        public ICompressor Create(WebPageContext context)
        {
            var value = context.GetConfigValue<string>("Page", "compressor", string.Empty).ToLower();
            if(!string.IsNullOrEmpty(value))
            {
                switch(value)
                {
                    case "gzip": return HttpCompressor.Instance;
                    default: return NonCompressor.Instance;
                }
            }

            //如果没有页面级配置，那么根据扩展名查看配置
            ICompressor compressor = null;
            if (_compressors.TryGetValue(context.PathExtension, out compressor))
                return compressor;

            return NonCompressor.Instance;
        }
    }
}
